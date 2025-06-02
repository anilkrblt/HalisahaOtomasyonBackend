using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Entities.Models;
using Shared.DataTransferObjects;

namespace HalisahaOtomasyon.AutoMap
{
    public class MappingProfile : Profile
    {


        public MappingProfile()
        {
            // Service/MappingProfile.cs

            // Mevcut mapping’lerinize ekleyin:
            CreateMap<FacilityRating, FacilityRatingDto>();
            CreateMap<FacilityRatingForCreationDto, FacilityRating>()
                .ForMember(dest => dest.Stars, opt => opt.MapFrom(src => src.Stars))
                .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment));
            CreateMap<FacilityRatingForUpdateDto, FacilityRating>()
                .ForMember(dest => dest.Stars, opt => opt.MapFrom(src => src.Stars))
                .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment));


            // AutoMap/MappingProfile.cs  (ilgili kısım)
            CreateMap<Team, TeamDto>()
                .ForMember(dest => dest.Members,
                             opt => opt.MapFrom(src => src.Members));


            CreateMap<TeamMember, TeamMemberDto>();   // üye map’i

            CreateMap<Team, TeamDto>()                // takım map’i
                .ForMember(d => d.Members,
                           opt => opt.MapFrom(s => s.Members));   // navigation dâhil



            CreateMap<WeeklyOpening, WeeklyOpeningDto>().ReverseMap();
            CreateMap<WeeklyOpeningForCreationDto, WeeklyOpening>();

            CreateMap<FieldException, FieldExceptionDto>().ReverseMap();
            CreateMap<FieldExceptionForCreationDto, FieldException>();


            CreateMap<Field, FieldDto>();
            CreateMap<FieldForCreationDto, Field>();
            CreateMap<FieldForUpdateDto, Field>();

            CreateMap<WeeklyOpening, WeeklyOpeningDto>().ReverseMap();
            CreateMap<WeeklyOpeningForCreationDto, WeeklyOpening>();

            CreateMap<FieldException, FieldExceptionDto>().ReverseMap();
            CreateMap<FieldExceptionForCreationDto, FieldException>();



            CreateMap<Facility, FacilityDto>()
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

            CreateMap<Reservation, ReservationDto>()
                .ForMember(d => d.SlotEnd,
                           opt => opt.MapFrom(s => s.SlotEnd));

            CreateMap<ReservationForCreationDto, Reservation>()
                .ForMember(d => d.Status, opt => opt.Ignore())   // varsayılan PendingOpponent
                .ForMember(d => d.CreatedAt, opt => opt.Ignore());



            CreateMap<ReservationParticipant, ReservationParticipantDto>();
            CreateMap<ReservationParticipantForCreationDto, ReservationParticipant>();

            CreateMap<MonthlyMembership, MonthlyMembershipDto>();
            CreateMap<MonthlyMembershipForCreationDto, MonthlyMembership>();


            CreateMap<Friendship, FriendshipDto>();





            CreateMap<FacilityRating, FacilityRatingDto>();
            CreateMap<FacilityRatingForCreationDto, FacilityRating>();



            CreateMap<Match, MatchDto>().ReverseMap();
            CreateMap<MatchForCreationDto, Match>();

            CreateMap<MatchRequestForCreationDto, MatchRequest>();
            CreateMap<MatchRequest, MatchRequestDto>();




            CreateMap<Team, TeamDto>().ReverseMap();
            CreateMap<TeamForCreationDto, Team>();
            CreateMap<TeamForUpdateDto, Team>();

            CreateMap<TeamMember, TeamMemberDto>();

            CreateMap<TeamJoinRequest, TeamJoinRequestDto>();
            CreateMap<TeamJoinRequestForCreationDto, TeamJoinRequest>();



            CreateMap<FieldCommentForCreationDto, FieldComment>();
            CreateMap<FieldComment, FieldCommentDto>();

            CreateMap<TeamCommentForCreationDto, TeamComment>();
            CreateMap<TeamComment, TeamCommentDto>();

            CreateMap<UserCommentForCreationDto, UserComment>();
            CreateMap<UserComment, UserCommentDto>();
            // -----------------------------------------------------








            CreateMap<Notification, NotificationDto>().ReverseMap();
            CreateMap<NotificationForCreationDto, Notification>();


            CreateMap<Reservation, ReservationDto>();
            CreateMap<ReservationForCreationDto, Reservation>();




            CreateMap<FacilityForCreationDto, Facility>();


            CreateMap<FacilityForUpdateDto, Facility>();


            CreateMap<Field, FieldDto>()
            .ForMember(
                dest => dest.Reservations,
                opt => opt.MapFrom(src => src.Reservations));

            CreateMap<FieldForCreationDto, Field>();
            CreateMap<FieldForUpdateDto, Field>();




            CreateMap<Equipment, EquipmentDto>();
            CreateMap<EquipmentForCreationDto, Equipment>();
            CreateMap<EquipmentForUpdateDto, Equipment>();


            CreateMap<Photo, PhotoDto>();
            CreateMap<PhotoForCreationDto, Photo>();

            CreateMap<Announcement, AnnouncementDto>();
            CreateMap<AnnouncementForCreationDto, Announcement>();
            CreateMap<AnnouncementForUpdateDto, Announcement>();
            CreateMap<AnnouncementForUpdateDto, Announcement>().ReverseMap();



            CreateMap<MatchForCreationDto, Match>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
                .ForMember(dest => dest.HomeScore, opt => opt.MapFrom(_ => 0))
                .ForMember(dest => dest.AwayScore, opt => opt.MapFrom(_ => 0));


            CreateMap<MatchForUpdateDto, Match>();





        }
    }
}
