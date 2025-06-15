using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace HalisahaOtomasyonPresentation.Controllers
{

    [Route("api/announcements")]
    [ApiController]
    public class AnnouncementController : ControllerBase
    {
        private readonly IServiceManager _service;
        public AnnouncementController(IServiceManager service)
        {
            _service = service;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAnnouncements([FromQuery] int? facilityId)
        {
            var anns = await _service.AnnouncementService
                                     .GetAllAnnouncementsAsync(trackChanges: false);

            if (facilityId.HasValue)
                anns = anns
                    .Where(a => a.FacilityId == facilityId.Value)
                    .ToList();

            foreach (var ann in anns)
            {
                var photos = await _service.PhotoService.GetPhotosAsync("announcement", ann.Id, false);
                var photo = photos.FirstOrDefault();
                ann.BannerUrl = photo?.Url;
            }

            return Ok(anns);
        }

        [HttpGet("{announcementId:int}", Name = "GetAnnouncement")]
        public async Task<IActionResult> GetAnnouncement(int announcementId)
        {
            var ann = await _service.AnnouncementService.GetAnnouncementAsync(announcementId, trackChanges: false);
            var photos = await _service.PhotoService.GetPhotosAsync("announcement", announcementId, false);
            var photo = photos.FirstOrDefault();
            ann.BannerUrl = photo?.Url;
            return Ok(ann);
        }

        [HttpPost]
        [Route("/api/announcements/{facilityId:int}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateAnnouncement([FromRoute] int facilityId, [FromForm] AnnouncementForCreationDto dto)
        {
            if (dto is null) return BadRequest();

            if (dto.PhotoFile?.Count > 1)
                return BadRequest("En fazla 1 fotoğraf yükleyebilirsiniz.");

            var created = await _service.AnnouncementService.CreateAnnouncementAsync(facilityId, dto);

            if (dto.PhotoFile?.Count > 0)
                await _service.PhotoService.UploadPhotosAsync(dto.PhotoFile, "announcement",
                                                            created.Id, facilityId);

            var photoDtos = await _service.PhotoService.GetPhotosAsync("announcement",
                                                                    created.Id, false);
            created.BannerUrl = photoDtos.Select(p => p.Url).FirstOrDefault();

            return CreatedAtAction(nameof(GetAnnouncement),
                                new { announcementId = created.Id, facilityId },
                                created);
        }

        [HttpPut("{announcementId:int}")]
        public async Task<IActionResult> UpdateAnnouncement(int announcementId, [FromBody] AnnouncementForUpdateDto dto)
        {
            if (dto is null) return BadRequest();

            await _service.AnnouncementService.UpdateAnnouncement(announcementId, dto, true);
            return NoContent();
        }

        [HttpDelete("{announcementId:int}")]
        public async Task<IActionResult> DeleteAnnouncement(int announcementId)
        {
            await _service.AnnouncementService.DeleteAnnouncement(announcementId, trackchanges: false);
            return NoContent();
        }

        [HttpPut("{id:int}/photos")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateAnnouncementPhotos(int id, [FromForm] FacilityPhotosUpdateDto dto)
        {
            if (dto.PhotoFiles == null || dto.PhotoFiles.Count == 0)
                return BadRequest("Fotoğraf yüklenmedi.");

            if (dto.PhotoFiles.Count > 1)
                return BadRequest("En fazla 1 fotoğraf yükleyebilirsiniz.");

            await _service.PhotoService.DeletePhotosByEntityAsync("announcement", id, trackChanges: true);

            await _service.PhotoService.UploadPhotosAsync(dto.PhotoFiles, "announcement", id);

            return NoContent();
        }
    }
}
