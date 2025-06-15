using AutoMapper;
using Entities.Models;
using Shared.DataTransferObjects;

namespace HalisahaOtomasyon.AutoMap
{
    public class EquipmentMappingProfile : Profile
    {
        public EquipmentMappingProfile()
        {
            CreateMap<Equipment, EquipmentDto>();
            CreateMap<EquipmentForCreationDto, Equipment>();
            CreateMap<EquipmentForUpdateDto, Equipment>();
        }
    }
}
