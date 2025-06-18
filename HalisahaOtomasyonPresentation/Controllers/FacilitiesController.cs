using HalisahaOtomasyon.ActionFilters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;
using System.Security.Claims;

namespace HalisahaOtomasyonPresentation.Controllers
{
    [ApiController]
    [Route("api/facilities")]
    public class FacilitiesController : ControllerBase
    {
        private readonly IServiceManager _service;
        public FacilitiesController(IServiceManager service)
        {
            _service = service;
        }

        [HttpGet]
        // [SwaggerOperation(Summary = "Tüm tesisleri getirir", Description = "Kayıtlı tüm tesisleri ve fotoğraflarını listeler.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFacilities([FromQuery] int? ownerId = null)
        {
            var facilities = await _service.FacilityService.GetAllFacilitiesAsync(ownerId, trackChanges: false);

            return Ok(facilities);
        }

        [HttpGet("{facilityId:int}", Name = "GetFacilityById")]
        // [SwaggerOperation(Summary = "Belirli bir tesisi getirir", Description = "ID’ye göre tesisi ve fotoğraflarını döner.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFacility([FromRoute(Name = "facilityId")] int facilityId)
        {
            var facility = await _service.FacilityService.GetFacilityAsync(facilityId, trackChanges: false);

            return Ok(facility);
        }

        [Authorize(Roles = "Owner")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [HttpPost]
        [Consumes("multipart/form-data")]
        //  [SwaggerOperation(Summary = "Yeni bir tesis oluşturur", Description = "Tesis bilgilerini ve maksimum 3 fotoğraf içeren bir form ile yeni tesis kaydı yapılır.")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateFacility([FromForm] FacilityForCreationDto dto)
        {
            var createdFacility = await _service.FacilityService.CreateFacilityAsync(dto);

            return CreatedAtRoute("GetFacilityById", new { facilityId = createdFacility.Id }, createdFacility);
        }

        [Authorize(Roles = "Owner")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [HttpPatch("{facilityId:int}")]
        // [SwaggerOperation(Summary = "Tesisin bazı alanlarını günceller", Description = "Kısmi veri gönderimiyle bir tesisin seçilen alanlarını günceller.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PatchFacility([FromRoute(Name = "facilityId")] int facilityId, 
            [FromBody] FacilityPatchDto patch)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("id")?.Value;
            if (userIdClaim is null)
                return Unauthorized();

            var reviewerId = int.Parse(userIdClaim);

            await _service.FacilityService.PatchFacilityAsync(reviewerId, facilityId, patch);
            return NoContent();
        }

        [Authorize(Roles = "Owner")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [HttpPut("{facilityId:int}/photos")]
        [Consumes("multipart/form-data")]
        // [SwaggerOperation(Summary = "Tesisin fotoğraflarını günceller", Description = "Önceki fotoğrafları siler, yerine yenilerini yükler (en fazla 3 adet).")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateFacilityPhotos([FromRoute(Name = "facilityId")] int facilityId,
            [FromForm] FacilityPhotosUpdateDto dto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
            ?? User.FindFirst("id")?.Value;
            if (userIdClaim is null)
                return Unauthorized();

            var reviewerId = int.Parse(userIdClaim);

            await _service.FacilityService.UpdateFacilityPhotos(reviewerId, facilityId, dto);
            return NoContent();
        }

        [Authorize(Roles = "Owner")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        [HttpPut("{facilityId:int}")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateFacility([FromRoute(Name = "facilityId")] int facilityId,
        [FromForm] FacilityForUpdateDto facilityDto)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                        ?? User.FindFirst("id")?.Value;
            if (userIdClaim is null)
                return Unauthorized();

            var reviewerId = int.Parse(userIdClaim);

            await _service.FacilityService.UpdateFacilityAsync(reviewerId, facilityId, facilityDto, true);
            return NoContent();
        }

        [Authorize(Roles = "Owner")]
        [HttpDelete("{facilityId:int}")]
        // [SwaggerOperation(Summary = "Tesis siler", Description = "ID’ye göre tesisi ve ona ait tüm fotoğrafları siler.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteFacility([FromRoute(Name = "facilityId")] int facilityId)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                    ?? User.FindFirst("id")?.Value;
            if (userIdClaim is null)
                return Unauthorized();

            var reviewerId = int.Parse(userIdClaim);

            await _service.FacilityService.DeleteFacility(reviewerId, facilityId, false);
            return NoContent();
        }
    }
}

