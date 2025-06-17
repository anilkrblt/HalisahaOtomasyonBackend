using AutoMapper;
using Entities.Models;
using Shared.DataTransferObjects;

namespace HalisahaOtomasyon.AutoMap
{
    public class FacilityMappingProfile : Profile
    {
        public FacilityMappingProfile()
        {
            CreateMap<Facility, FacilityDto>()
                .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => src.OwnerId))
                .ForMember(d => d.Fields,
                           opt => opt.MapFrom(src => src.Fields))
                .ForMember(d => d.Equipments,
                           opt => opt.MapFrom(src => src.Equipments))
                .ForMember(d => d.PhotoUrls,
                           opt => opt.MapFrom(src => src.Photos.Select(p => p.Url)))
                .ForMember(d => d.HasShoeRental,
                           opt => opt.MapFrom(src =>
                               src.Equipments.Any(e => e.Name.ToLower().Contains("ayakkabı") && e.IsRentable)))
                .ForMember(d => d.HasGlove,
                           opt => opt.MapFrom(src =>
                               src.Equipments.Any(e => e.Name.ToLower().Contains("eldiven") && e.IsRentable)))
                .ForMember(d => d.HasCamera,
                           opt => opt.MapFrom(src =>
                               src.Fields.Any(f => f.HasCamera)));

            CreateMap<FacilityForCreationDto, Facility>();
            CreateMap<FacilityForUpdateDto, Facility>().ReverseMap();
            CreateMap<FacilityPatchDto, Facility>().ReverseMap();

            CreateMap<Facility, FacilityDto>()
                .ForMember(dest => dest.Fields, opt => opt.MapFrom(src => src.Fields))
                .ForMember(dest => dest.Equipments, opt => opt.MapFrom(src => src.Equipments));

            CreateMap<FacilityForCreationDto, Facility>();
            CreateMap<FacilityForUpdateDto, Facility>().ReverseMap();

            CreateMap<FacilityForCreationDto, Facility>();
            CreateMap<FacilityForUpdateDto, Facility>();
        }
    }
}
