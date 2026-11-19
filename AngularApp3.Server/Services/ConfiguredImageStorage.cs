using AngularApp3.Server.Common;
using AngularApp3.Server.Configuration;
using AngularApp3.Server.Services.Interfaces;
using Microsoft.Extensions.Options;

namespace AngularApp3.Server.Services;

public class ConfiguredImageStorage : IImageStorage
{
    private readonly SupabaseImageStorage _remote;
    private readonly LocalImageStore _local;
    private readonly SupabaseStorageOptions _options;

    public ConfiguredImageStorage(
        SupabaseImageStorage remote,
        LocalImageStore local,
        IOptions<SupabaseStorageOptions> options)
    {
        _remote = remote;
        _local = local;
        _options = options.Value;
    }

    public Task<ServiceResult<string>> UploadAsync(
        Stream content,
        string contentType,
        string fileName,
        CancellationToken cancellationToken = default)
    {
        if (IsRemoteConfigured)
        {
            return _remote.UploadAsync(content, contentType, fileName, cancellationToken);
        }

        return _local.SaveAsync(content, contentType, fileName, cancellationToken);
    }

    private bool IsRemoteConfigured =>
        !string.IsNullOrWhiteSpace(_options.Url) &&
        !string.IsNullOrWhiteSpace(_options.ServiceRoleKey) &&
        !string.IsNullOrWhiteSpace(_options.Bucket);
}
