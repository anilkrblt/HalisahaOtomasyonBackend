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
    private readonly IFacilityRatingService _svc;
    public FacilityRatingsController(IServiceManager sm)
        => _svc = sm.FacilityRatingService;

    [HttpGet]
    public async Task<IActionResult> GetRatings(int facilityId)
        => Ok(await _svc.GetRatingsByFacilityAsync(facilityId, false));

    [HttpGet("average")]
    public async Task<IActionResult> GetAverage(int facilityId)
        => Ok(await _svc.GetAverageStarsAsync(facilityId));

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
        var created = await _svc.AddRatingAsync(facilityId, dto, userId);

        return CreatedAtAction(
            nameof(GetRatings),
            new { facilityId },
            created);
    }

    [HttpPut]
    [Authorize]
    public async Task<IActionResult> UpdateRating(
        int facilityId,
        [FromBody] FacilityRatingForUpdateDto dto)
    {


        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? User.FindFirst("id")?.Value;
        if (userIdClaim is null) return Unauthorized();

        var userId = int.Parse(userIdClaim);

        await _svc.UpdateRatingAsync(facilityId, userId, dto);
        return NoContent();
    }


    /*
        [HttpGet("/api/users/{userId:int}/rated-facilities")]
        [Authorize]
        public async Task<IActionResult> GetUserRatedFacilities(int userId)
        {
            var tokenUser = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value!);
            if (tokenUser != userId) return Forbid();

            var list = await _svc.GetRatedFacilitiesByUserAsync(userId, false);
            return Ok(list);
        }
        */
}
