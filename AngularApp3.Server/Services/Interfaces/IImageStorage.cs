using AngularApp3.Server.Common;

namespace AngularApp3.Server.Services.Interfaces;

public interface IImageStorage
{
    Task<ServiceResult<string>> UploadAsync(
        Stream content,
        string contentType,
        string fileName,
        CancellationToken cancellationToken = default);
}
