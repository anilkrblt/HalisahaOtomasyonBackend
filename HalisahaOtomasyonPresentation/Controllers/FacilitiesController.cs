using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace HalisahaOtomasyonPresentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
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
            var facilities = await _service.FacilityService.GetAllFacilitiesAsync(trackChanges: false);

            var filteredFacilities = ownerId.HasValue ? facilities.Where(f => f.OwnerId == ownerId.Value).ToList()
                                                     : facilities.ToList();
            foreach (var facility in filteredFacilities)
            {
                var photoDtos = await _service.PhotoService.GetPhotosAsync("facility", facility.Id, false);
                facility.PhotoUrls = photoDtos.Select(p => p.Url).ToList();
            }

            return Ok(filteredFacilities);

        }

        [HttpGet("{id:int}", Name = "GetFacilityById")]
        // [SwaggerOperation(Summary = "Belirli bir tesisi getirir", Description = "ID’ye göre tesisi ve fotoğraflarını döner.")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFacility(int id)
        {
            var facility = await _service.FacilityService.GetFacilityAsync(id, trackChanges: false);
            var photoDtos = await _service.PhotoService.GetPhotosAsync("facility", facility.Id, false);
            var photoUrls = photoDtos.Select(p => p.Url).ToList();
            facility.PhotoUrls = photoUrls;

            return Ok(facility);
        }


        //[ServiceFilter(typeof(ValidationFilterAttribute))]
        [HttpPost]
        [Consumes("multipart/form-data")]
        //  [SwaggerOperation(Summary = "Yeni bir tesis oluşturur", Description = "Tesis bilgilerini ve maksimum 3 fotoğraf içeren bir form ile yeni tesis kaydı yapılır.")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateFacility([FromForm] FacilityForCreationDto dto)
        {
            if (dto == null)
                return BadRequest("Facility DTO is null.");

            if (dto.PhotoFiles != null && dto.PhotoFiles.Count > 3)
                return BadRequest("En fazla 3 fotoğraf yükleyebilirsiniz.");


            var createdFacility = await _service.FacilityService.CreateFacilityAsync(dto);

            // 2. Fotoğrafları PhotoService'e gönder
            if (dto.PhotoFiles is not null && dto.PhotoFiles.Count > 0)
            {
                await _service.PhotoService.UploadPhotosAsync(dto.PhotoFiles, "facility", createdFacility.Id);
            }

            return CreatedAtRoute("GetFacilityById", new { id = createdFacility.Id }, createdFacility);
        }




        [HttpDelete("{id:int}")]
        // [SwaggerOperation(Summary = "Tesis siler", Description = "ID’ye göre tesisi ve ona ait tüm fotoğrafları siler.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteFacility(int id)
        {
            await _service.PhotoService.DeletePhotosByEntityAsync("facility", id, trackChanges: true);
            await _service.FacilityService.DeleteFacility(id, false);
            return NoContent();
        }


        [HttpPut("{id:int}")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateFacility(
     int id,
     [FromForm] FacilityForUpdateDto facilityDto)
        {
            await _service.FacilityService.UpdateFacilityAsync(id, facilityDto, true);
            return NoContent();
        }


        [HttpPatch("{id:int}")]
        // [SwaggerOperation(Summary = "Tesisin bazı alanlarını günceller", Description = "Kısmi veri gönderimiyle bir tesisin seçilen alanlarını günceller.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PatchFacility(int id, [FromBody] FacilityPatchDto patch)
        {
            if (patch == null)
                return BadRequest("Gönderilen veri boş olamaz.");
            await _service.FacilityService.PatchFacilityAsync(id, patch);
            return NoContent();
        }

        // fotoğraf güncelleme

        [HttpPut("{id:int}/photos")]
        [Consumes("multipart/form-data")]
        // [SwaggerOperation(Summary = "Tesisin fotoğraflarını günceller", Description = "Önceki fotoğrafları siler, yerine yenilerini yükler (en fazla 3 adet).")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateFacilityPhotos(int id, [FromForm] FacilityPhotosUpdateDto dto)
        {
            if (dto.PhotoFiles == null || dto.PhotoFiles.Count == 0)
                return BadRequest("Fotoğraf yüklenmedi.");

            if (dto.PhotoFiles.Count > 3)
                return BadRequest("En fazla 3 fotoğraf yükleyebilirsiniz.");

            await _service.PhotoService.DeletePhotosByEntityAsync("facility", id, trackChanges: true);

            await _service.PhotoService.UploadPhotosAsync(dto.PhotoFiles, "facility", id);

            return NoContent();
        }

    }
}

