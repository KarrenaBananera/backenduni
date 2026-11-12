using System.Text.RegularExpressions;
using AngularApp3.Server.Common;

namespace AngularApp3.Server.Services;

public sealed class LocalImageStore
{
    private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg",
        "image/png",
        "image/webp"
    };

    private static readonly Regex SafeName = new(
        @"^[a-f0-9]{32}\.(jpg|jpeg|png|webp)$",
        RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Compiled);

    private readonly string _directory;

    public LocalImageStore(IWebHostEnvironment environment)
    {
        _directory = Path.Combine(environment.ContentRootPath, "App_Data", "images");
    }

    public static bool IsSafeName(string name) => SafeName.IsMatch(name);

    private static bool IsAllowedExtension(string extension) =>
        extension.Equals(".jpg", StringComparison.OrdinalIgnoreCase) ||
        extension.Equals(".jpeg", StringComparison.OrdinalIgnoreCase) ||
        extension.Equals(".png", StringComparison.OrdinalIgnoreCase) ||
        extension.Equals(".webp", StringComparison.OrdinalIgnoreCase);

    public async Task<ServiceResult<string>> SaveAsync(
        Stream content,
        string contentType,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        if (!AllowedContentTypes.Contains(contentType))
        {
            return ServiceResult<string>.Failure(
                "Only jpeg, png and webp images are allowed.",
                StatusCodes.Status400BadRequest);
        }

        var extension = Path.GetExtension(fileName);
        if (!IsAllowedExtension(extension))
        {
            extension = contentType switch
            {
                "image/png" => ".png",
                "image/webp" => ".webp",
                _ => ".jpg"
            };
        }

        var name = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        Directory.CreateDirectory(_directory);
        var path = Path.Combine(_directory, name);
        await using (var output = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None))
        {
            await content.CopyToAsync(output, cancellationToken);
        }

        return ServiceResult<string>.Success($"/api/images/{name}");
    }

    public bool TryOpen(string name, out FileStream stream, out string contentType)
    {
        stream = null!;
        contentType = string.Empty;
        if (!IsSafeName(name))
        {
            return false;
        }

        var path = Path.Combine(_directory, name);
        if (!File.Exists(path))
        {
            return false;
        }

        contentType = Path.GetExtension(name).ToLowerInvariant() switch
        {
            ".png" => "image/png",
            ".webp" => "image/webp",
            _ => "image/jpeg"
        };
        stream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
        return true;
    }
}
