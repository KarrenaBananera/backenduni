using AngularApp3.Server.Identity;
using AngularApp3.Server.Services;
using AngularApp3.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AngularApp3.Server.Controllers;

[ApiController]
[Route("api/images")]
public class ImagesController : ControllerBase
{
    private const long MaxFileSize = 5 * 1024 * 1024;

    private readonly IImageStorage _imageStorage;
    private readonly LocalImageStore _localImages;

    public ImagesController(IImageStorage imageStorage, LocalImageStore localImages)
    {
        _imageStorage = imageStorage;
        _localImages = localImages;
    }

    [Authorize(Roles = AppRoles.Admin)]
    [HttpPost]
    [RequestSizeLimit(MaxFileSize)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxFileSize)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Upload(IFormFile file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(new { error = "Image file is required." });
        }

        if (file.Length > MaxFileSize)
        {
            return BadRequest(new { error = "Image must be 5 MB or smaller." });
        }

        await using var stream = file.OpenReadStream();
        var result = await _imageStorage.UploadAsync(stream, file.ContentType, file.FileName, cancellationToken);
        if (!result.Succeeded)
        {
            return StatusCode(result.StatusCode, new { error = result.Error });
        }

        return Ok(new { url = result.Data });
    }

    [HttpGet("{name}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Get(string name)
    {
        if (!_localImages.TryOpen(name, out var stream, out var contentType))
        {
            return NotFound();
        }

        return File(stream, contentType);
    }
}
