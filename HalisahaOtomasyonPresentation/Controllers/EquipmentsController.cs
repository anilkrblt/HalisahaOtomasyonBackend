using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace HalisahaOtomasyonPresentation.Controllers
{
    [ApiController]
    [Route("api/facilities/{facilityId}/equipments")]
    public class EquipmentsController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        public EquipmentsController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetEquipments(int facilityId)
        {
            var equipments = await _serviceManager.EquipmentService.GetAllEquipmentsByFacilityAsync(facilityId, trackChanges: false);


            foreach (var equipment in equipments)
            {
                var photoDtos = await _serviceManager.PhotoService.GetPhotosAsync("equipment", equipment.Id, false);
                equipment.PhotoUrls = photoDtos.Select(p => p.Url).ToList();
            }


            return Ok(equipments);
        }

        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreateEquipment(int facilityId, [FromForm] EquipmentForCreationDto dto)
        {
            var equipment = await _serviceManager.EquipmentService.CreateEquipmentAsync(dto, facilityId);


            if (dto.PhotoFiles is not null && dto.PhotoFiles.Count > 0)
            {

                await _serviceManager.PhotoService.UploadPhotosAsync(dto.PhotoFiles, "equipment", equipment.Id, facilityId);
            }
            return StatusCode(201);
        }

        [HttpPut("{equipmentId}")]
        public async Task<IActionResult> UpdateEquipment(int equipmentId, [FromBody] EquipmentForUpdateDto equipmentDto)
        {
            await _serviceManager.EquipmentService.UpdateEquipmentAsync(equipmentId, equipmentDto, trackChanges: true);

            return NoContent();
        }


        [HttpPut("{id:int}/photos")]
        [Consumes("multipart/form-data")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateEquipmentPhotos(int id, [FromForm] FacilityPhotosUpdateDto dto)
        {
            if (dto.PhotoFiles == null || dto.PhotoFiles.Count == 0)
                return BadRequest("Fotoğraf yüklenmedi.");

            if (dto.PhotoFiles.Count > 3)
                return BadRequest("En fazla 3 fotoğraf yükleyebilirsiniz.");

            // Önce eski fotoğrafları sil
            await _serviceManager.PhotoService.DeletePhotosByEntityAsync("equipment", id, trackChanges: true);

            // Yeni fotoğrafları yükle
            await _serviceManager.PhotoService.UploadPhotosAsync(dto.PhotoFiles, "equipment", id);

            return NoContent();
        }



        [HttpDelete("{equipmentId}")]
        public async Task<IActionResult> DeleteEquipment(int equipmentId)
        {
            await _serviceManager.PhotoService.DeletePhotosByEntityAsync("equipment", equipmentId, trackChanges: true);
            await _serviceManager.EquipmentService.DeleteEquipmentAsync(equipmentId, trackChanges: true);


            return NoContent();
        }
    }
}