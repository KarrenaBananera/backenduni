using AngularApp3.Server.DTOs.PromoCodes;
using AngularApp3.Server.Identity;
using AngularApp3.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AngularApp3.Server.Controllers;

[ApiController]
[Authorize(Roles = AppRoles.Admin)]
[Route("api/promo-codes")]
public class PromoCodesController : ControllerBase
{
    private readonly IPromoCodeService _promoCodeService;

    public PromoCodesController(IPromoCodeService promoCodeService)
    {
        _promoCodeService = promoCodeService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PromoCodeDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var promoCodes = await _promoCodeService.GetAllAsync(cancellationToken);
        return Ok(promoCodes);
    }

    [HttpPost]
    [ProducesResponseType(typeof(PromoCodeDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Create([FromBody] CreatePromoCodeRequest request, CancellationToken cancellationToken)
    {
        var result = await _promoCodeService.CreateAsync(request, cancellationToken);
        if (!result.Succeeded)
        {
            return StatusCode(result.StatusCode, new { error = result.Error });
        }

        return StatusCode(result.StatusCode, result.Data);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var deleted = await _promoCodeService.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            return NotFound();
        }

        return NoContent();
    }
}
