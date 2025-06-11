using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;
using HalisahaOtomasyon.ActionFilters;
using Microsoft.AspNetCore.Http;

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
        public async Task<IActionResult> GetFields([FromQuery] int? facilityId)
        {
            // 1) Tüm sahaları çek (facilityId filtreli de olabilir)
            var allFields = await _serviceManager
                                     .FieldService
                                     .GetAllFieldsAsync(trackChanges: false);

            // 2) Eğer facilityId verilmişse, sadece o tesise ait sahaları al
            var fields = facilityId.HasValue
                ? allFields.Where(f => f.FacilityId == facilityId.Value).ToList()
                : allFields.ToList();

            // 3) Her bir saha için foto URL’lerini ekle
            foreach (var field in fields)
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


        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteField(int id)
        {

            await _serviceManager.PhotoService.DeletePhotosByEntityAsync("field", id, trackChanges: false);
            await _serviceManager.FieldService.DeleteFieldAsync(id);

            return NoContent();
        }

        [HttpPut("{id:int}")]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> UpdateField(int id, [FromBody] FieldForUpdateDto field)
        {
            await _serviceManager.FieldService.UpdateFieldAsync(id, field, true);
            return NoContent();
        }
        
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> PatchField(int id, [FromBody] FieldPatchDto patch)
        {
            if (patch == null)
                return BadRequest("Gönderilen veri boş olamaz.");
            await _serviceManager.FieldService.PatchFieldAsync(id, patch);
            return NoContent();
        }


        [HttpPut("{id:int}/photos")]
        [Consumes("multipart/form-data")]
        // [SwaggerOperation(Summary = "Tesisin fotoğraflarını günceller", Description = "Önceki fotoğrafları siler, yerine yenilerini yükler (en fazla 3 adet).")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateFieldPhotos(int id, [FromForm] FieldPhotosUpdateDto dto)
        {
            if (dto.PhotoFiles == null || dto.PhotoFiles.Count == 0)
                return BadRequest("Fotoğraf yüklenmedi.");

            if (dto.PhotoFiles.Count > 3)
                return BadRequest("En fazla 3 fotoğraf yükleyebilirsiniz.");

            await _serviceManager.PhotoService.DeletePhotosByEntityAsync("field", id, trackChanges: true);

            // Yeni fotoğrafları yükle
            await _serviceManager.PhotoService.UploadPhotosAsync(dto.PhotoFiles, "field", id);

            return NoContent();
        }

    }
}
