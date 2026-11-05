using AngularApp3.Server.Common;
using AngularApp3.Server.Configuration;
using AngularApp3.Server.Services.Interfaces;
using Microsoft.Extensions.Options;
using Supabase.Storage;
using Supabase.Storage.Exceptions;

namespace AngularApp3.Server.Services;

public class SupabaseImageStorage : IImageStorage
{
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp"
    };

    private readonly SupabaseStorageOptions _options;

    public SupabaseImageStorage(IOptions<SupabaseStorageOptions> options)
    {
        _options = options.Value;
    }

    public async Task<ServiceResult<string>> UploadAsync(
        Stream content,
        string contentType,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(_options.Url) || string.IsNullOrWhiteSpace(_options.ServiceRoleKey))
        {
            return ServiceResult<string>.Failure(
                "Supabase storage is not configured.",
                StatusCodes.Status503ServiceUnavailable);
        }

        if (!AllowedContentTypes.Contains(contentType))
        {
            return ServiceResult<string>.Failure(
                "Only jpeg, png and webp images are allowed.",
                StatusCodes.Status400BadRequest);
        }

        var extension = Path.GetExtension(fileName);
        if (string.IsNullOrWhiteSpace(extension))
        {
            extension = contentType switch
            {
                "image/png" => ".png",
                "image/webp" => ".webp",
                _ => ".jpg"
            };
        }

        var objectName = $"{Guid.NewGuid():N}{extension}";
        var projectUrl = _options.Url.TrimEnd('/');
        var bucketName = string.IsNullOrWhiteSpace(_options.Bucket) ? "products" : _options.Bucket.Trim('/');
        var client = new Client($"{projectUrl}/storage/v1", new Dictionary<string, string>
        {
            ["apikey"] = _options.ServiceRoleKey,
            ["Authorization"] = $"Bearer {_options.ServiceRoleKey}"
        });
        var bucket = client.From(bucketName);

        using var memory = new MemoryStream();
        await content.CopyToAsync(memory, cancellationToken);

        try
        {
            await bucket.Upload(
                memory.ToArray(),
                objectName,
                new Supabase.Storage.FileOptions(),
                onProgress: null,
                inferContentType: true,
                cancellationToken: cancellationToken);
        }
        catch (SupabaseStorageException)
        {
            return ServiceResult<string>.Failure(
                "Image storage rejected the upload.",
                StatusCodes.Status502BadGateway);
        }

        return ServiceResult<string>.Success(bucket.GetPublicUrl(objectName, null));
    }
}
