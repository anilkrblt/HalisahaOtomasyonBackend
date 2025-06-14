﻿using AutoMapper;
using Entities.Models;
using Shared.DataTransferObjects;

namespace HalisahaOtomasyon.AutoMap
{
    public class TeamMappingProfile : Profile
    {
        public TeamMappingProfile()
        {
            CreateMap<Team, TeamDto>()
                .ForMember(dest => dest.Members, opt => opt.MapFrom(src => src.Members));
            CreateMap<TeamForCreationDto, Team>();
            CreateMap<TeamForUpdateDto, Team>();

            CreateMap<TeamMemberDtoForAdd, TeamMember>();
            CreateMap<TeamMember, TeamMemberDto>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.User.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.User.LastName))
                .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Position))
                .ForMember(dest => dest.IsCaptain, opt => opt.MapFrom(src => src.IsCaptain))
                .ForMember(dest => dest.IsAdmin, opt => opt.MapFrom(src => src.IsAdmin))
                .ForMember(dest => dest.JoinedAt, opt => opt.MapFrom(src => src.JoinedAt))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));
            CreateMap<TeamMemberDtoForUpdateAdminAndCaptain, TeamMember>();

            CreateMap<TeamJoinRequest, TeamJoinRequestDto>();
        }
    }
}
