using AutoMapper;
using Entities.Models;
using Shared.DataTransferObjects;

namespace HalisahaOtomasyon.AutoMap
{
    public class AnnouncementMappingProfile : Profile
    {
        public AnnouncementMappingProfile()
        {
            CreateMap<Announcement, AnnouncementDto>();
            CreateMap<AnnouncementForCreationDto, Announcement>();
            CreateMap<AnnouncementForUpdateDto, Announcement>();
            CreateMap<AnnouncementForUpdateDto, Announcement>().ReverseMap();
        }
    }
}
