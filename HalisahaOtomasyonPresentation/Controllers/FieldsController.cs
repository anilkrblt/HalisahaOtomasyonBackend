using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;
using HalisahaOtomasyon.ActionFilters;
using Microsoft.AspNetCore.Http;

/*
{

    "weeklyOpenings": [
      {
        "dayOfWeek": "Monday",
      "startTime": "09:00,
        "endTime": "23:00"
      },
    {
        "dayOfWeek": "Wednesday",
      "startTime": "07:00,
      "endTime": "21:00"
    }
  ],
  "exceptions": [
    {
        "date": "2025-06-15",
      "isOpen": false
    },
    {
        "date": "2025-06-16",
      "isOpen": false
    }
  ]
}

*/
namespace HalisahaOtomasyonPresentation.Controllers
{
    [ApiController]
    [Route("api/facilities/fields")]
    public class FieldsController : ControllerBase
    {

        private readonly IServiceManager _serviceManager;
        public FieldsController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetFields([FromQuery] int? facilityId, [FromQuery] int? ownerId)
        {
            // 1) Tüm sahaları çek (facilityId filtreli de olabilir)
            var allFields = await _serviceManager
                         .FieldService
                         .GetAllFieldsAsync(trackChanges: false);

            // 2) İsteğe göre filtrele
            var fields = allFields.AsQueryable();

            if (facilityId.HasValue)
                fields = fields.Where(f => f.FacilityId == facilityId.Value);

            if (ownerId.HasValue)
                fields = fields.Where(f => f.OwnerId == ownerId.Value);

            var fieldList = fields.ToList();


            // 3) Her bir saha için foto URL’lerini ekle
            foreach (var field in fieldList)
            {
                var photoDtos = await _serviceManager
                                      .PhotoService
                                      .GetPhotosAsync("field", field.Id, trackChanges: false);
                field.PhotoUrls = photoDtos.Select(p => p.Url).ToList();
            }

            return Ok(fields);
        }

        [HttpGet("{id:int}", Name = "FieldById")]
        public async Task<IActionResult> GetField(int id)
        {
            var field = await _serviceManager.FieldService.GetFieldAsync(id, trackChanges: false);
            var photoDtos = await _serviceManager.PhotoService.GetPhotosAsync("field", field.Id, false);
            field.PhotoUrls = photoDtos.Select(p => p.Url).ToList();

            return Ok(field);
        }

        [Consumes("multipart/form-data")]
        [HttpPost("{id}/photos")]
        public async Task<IActionResult> UploadPhotos(int id, [FromForm] FieldPhotosUpdateDto dto)
        {

            if (dto.PhotoFiles != null && dto.PhotoFiles.Count > 3)
                return BadRequest("En fazla 3 fotoğraf yükleyebilirsiniz.");


            if (dto.PhotoFiles is not null && dto.PhotoFiles.Count > 0)
            {

                await _serviceManager.PhotoService.UploadPhotosAsync(dto.PhotoFiles, "field", id);
            }

            return NoContent();
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateField([FromBody] FieldForCreationDto dto)
        {
            if (dto == null)
                return BadRequest("Field DTO is null.");

            var createdField = await _serviceManager.FieldService.CreateFieldAsync(dto);
            createdField.PhotoUrls ??= [];

            return CreatedAtRoute("FieldById", new { id = createdField.Id }, createdField);
        }

        [HttpPost("mobil")]
        [Consumes("multipart/form-data")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateFieldMobil([FromForm] FieldForCreationDto dto, List<IFormFile> PhotoFiles)
        {
            if (dto == null)
                return BadRequest("Field DTO is null.");

            var createdField = await _serviceManager.FieldService.CreateFieldAsync(dto);
            createdField.PhotoUrls ??= [];

            if (PhotoFiles != null && PhotoFiles.Count > 3)
                return BadRequest("En fazla 3 fotoğraf yükleyebilirsiniz.");


            if (PhotoFiles is not null && PhotoFiles.Count > 0)
            {

                await _serviceManager.PhotoService.UploadPhotosAsync(PhotoFiles, "field", createdField.Id);
            }



            return CreatedAtRoute("FieldById", new { id = createdField.Id }, createdField);
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteField(int id)
        {

            await _serviceManager.PhotoService.DeletePhotosByEntityAsync("field", id, trackChanges: false);
            await _serviceManager.FieldService.DeleteFieldAsync(id);

            return NoContent();
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateField(int id, [FromBody] FieldForUpdateDto field)
        {
            // Process the request
            await _serviceManager.FieldService.UpdateFieldAsync(id, field, true);
            return NoContent();
        }

        [HttpPatch("{id:int}")]
        public async Task<IActionResult> PatchField(int id, [FromBody] FieldPatchDto patch)
        {

            await _serviceManager.FieldService.PatchFieldAsync(id, patch);
            return NoContent();
        }

        [HttpPut("{id:int}/photos")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateFieldPhotos(int id, [FromForm] FieldPhotosUpdateDto dto)
        {
            if (dto.PhotoFiles == null || dto.PhotoFiles.Count == 0)
                return BadRequest("Fotoğraf yüklenmedi.");

            if (dto.PhotoFiles.Count > 3)
                return BadRequest("En fazla 3 fotoğraf yükleyebilirsiniz.");

            await _serviceManager.PhotoService.DeletePhotosByEntityAsync("field", id, trackChanges: true);

            await _serviceManager.PhotoService.UploadPhotosAsync(dto.PhotoFiles, "field", id);

            return NoContent();
        }
    }
}
