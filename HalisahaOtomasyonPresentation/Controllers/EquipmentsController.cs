using Microsoft.AspNetCore.Mvc;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace HalisahaOtomasyonPresentation.Controllers
{
    [ApiController]
    [Route("api/facilities/{facilityId:int}/equipments")]
    public class EquipmentsController : ControllerBase
    {
        private readonly IServiceManager _serviceManager;
        public EquipmentsController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetEquipments([FromRoute(Name = "facilityId")] int facilityId)
        {
            var equipments = await _serviceManager.EquipmentService.GetAllEquipmentsByFacilityAsync(facilityId, trackChanges: false);

            foreach (var equipment in equipments)
            {
                var photoDtos = await _serviceManager.PhotoService.GetPhotosAsync("equipment", equipment.Id, false);
                equipment.PhotoUrls = photoDtos.Select(p => p.Url).ToList();
            }

            return Ok(equipments);
        }

        [Consumes("multipart/form-data")]
        [HttpPost]
        public async Task<IActionResult> CreateEquipment([FromRoute(Name = "facilityId")] int facilityId, [FromForm] EquipmentForCreationDto dto)
        {
            var equipment = await _serviceManager.EquipmentService.CreateEquipmentAsync(dto, facilityId);

            if (dto.PhotoFiles is not null && dto.PhotoFiles.Count > 0)
            {
                await _serviceManager.PhotoService.UploadPhotosAsync(dto.PhotoFiles, "equipment", equipment.Id, facilityId);
            }

            return StatusCode(201);
        }

        [Consumes("multipart/form-data")]
        [HttpPut("{equipmentId:int}")]
        public async Task<IActionResult> UpdateEquipment([FromRoute(Name = "facilityId")] int facilityId, 
            [FromRoute(Name = "equipmentId")] int equipmentId, 
            [FromForm] EquipmentForUpdateDto equipmentDto)
        {
            await _serviceManager.EquipmentService.UpdateEquipmentAsync(equipmentId, equipmentDto, trackChanges: true);

            if (equipmentDto.PhotoFiles is not null && equipmentDto.PhotoFiles.Count > 0)
            {
                await _serviceManager.PhotoService.DeletePhotosByEntityAsync("equipment", equipmentId, trackChanges: true);
                await _serviceManager.PhotoService.UploadPhotosAsync(equipmentDto.PhotoFiles, "equipment", equipmentId, facilityId);
            }

            return NoContent();
        }

        [HttpDelete("{equipmentId:int}")]
        public async Task<IActionResult> DeleteEquipment([FromRoute(Name = "equipmentId")] int equipmentId)
        {
            await _serviceManager.PhotoService.DeletePhotosByEntityAsync("equipment", equipmentId, trackChanges: true);
            await _serviceManager.EquipmentService.DeleteEquipmentAsync(equipmentId, trackChanges: true);

            return NoContent();
        }
    }
}