using AngularApp3.Server.Identity;
using AngularApp3.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AngularApp3.Server.Controllers;

[ApiController]
[Authorize(Roles = AppRoles.Admin)]
[Route("api/images")]
public class ImagesController : ControllerBase
{
    private const long MaxFileSize = 5 * 1024 * 1024;

    private readonly IImageStorage _imageStorage;

    public ImagesController(IImageStorage imageStorage)
    {
        _imageStorage = imageStorage;
    }

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
}
