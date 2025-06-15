using AutoMapper;
using Entities.Models;
using Shared.DataTransferObjects;

namespace HalisahaOtomasyon.AutoMap
{
    public class MappingProfileForTeam : Profile
    {
        public MappingProfileForTeam()
        {
            CreateMap<TeamMemberDtoForAdd, TeamMember>();
        }
    }
}
