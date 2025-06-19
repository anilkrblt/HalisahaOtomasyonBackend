using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace HalisahaOtomasyonPresentation.Controllers;
// dummy commit
[ApiController]
[Route("api/facilities/{facilityId:int}/ratings")]
public class FacilityRatingsController : ControllerBase
{
    private readonly IServiceManager _svc;
    public FacilityRatingsController(IServiceManager sm)
        => _svc = sm;

    [HttpGet]
    public async Task<IActionResult> GetRatings(int facilityId)
        => Ok(await _svc.FacilityRatingService.GetRatingsByFacilityAsync(facilityId, false));

    [HttpGet("average")]
    public async Task<IActionResult> GetAverage(int facilityId)
        => Ok(await _svc.FacilityRatingService.GetAverageStarsAsync(facilityId));

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> AddRating(
        int facilityId,
        [FromBody] FacilityRatingForCreationDto dto)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                       ?? User.FindFirst("id")?.Value;
        if (userIdClaim is null) return Unauthorized();

        var userId = int.Parse(userIdClaim);
        var created = await _svc.FacilityRatingService.AddRatingAsync(facilityId, dto, userId);

        var facility = await _svc.FacilityService.GetFacilityAsync(facilityId, false);
        if (facility?.OwnerId is not null && facility.OwnerId != userId)
        {
            await _svc.NotificationService.CreateNotificationAsync(new NotificationForCreationDto
            {
                UserId = facility.OwnerId,
                Title = "Yeni Tesis Puanlaması",
                Content = "Bir kullanıcı tesisinize puan verdi.",
                RelatedId = created.UserId,
                RelatedType = "facility-rating"
            });
        }

        return CreatedAtAction(
            nameof(GetRatings),
            new { facilityId },
            created);
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateRating(int facilityId, [FromBody] FacilityRatingForUpdateDto dto)
    {

        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("id")?.Value;
        if (userIdClaim is null) return Unauthorized();

        var userId = int.Parse(userIdClaim);

        await _svc.FacilityRatingService.UpdateRatingAsync(facilityId, userId, dto);
        return NoContent();
    }
}
