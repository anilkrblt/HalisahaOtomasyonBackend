using AutoMapper;
using Entities.Models;
using Shared.DataTransferObjects;

namespace HalisahaOtomasyon.AutoMap
{
    public class MappingProfile : Profile
    {
        public static string? GetHomeTeamName(Room room)
        {
            var home = room.Participants.FirstOrDefault(p => p.IsHome);
            return home?.Team?.Name;
        }

        public static string? GetAwayTeamName(Room room)
        {
            var away = room.Participants.FirstOrDefault(p => !p.IsHome);
            return away?.Team?.Name;
        }

        public static int? GetHomeTeamId(Room room)
        {
            var home = room.Participants.FirstOrDefault(p => p.IsHome);
            return home?.Team?.Id;
        }

        public static int? GetAwayTeamId(Room room)
        {
            var away = room.Participants.FirstOrDefault(p => !p.IsHome);
            return away?.Team?.Id;
        }

        public MappingProfile()
        {
            CreateMap<Reservation, ReservationDto>().ReverseMap();

            CreateMap<ReservationForCreationDto, Reservation>();

            CreateMap<ReservationPayment, ReservationPaymentDto>().ReverseMap();

            CreateMap<ReservationPaymentForCreationDto, ReservationPayment>();

            CreateMap<RoomParticipant, RoomParticipantDto>();

            CreateMap<Room, RoomDto>()
            .ForMember(d => d.RoomId, opt => opt.MapFrom(src => src.Id))
            .ForMember(d => d.FieldName, opt => opt.MapFrom(src => src.Field.Name))
            .ForMember(d => d.HomeTeamName, opt => opt.MapFrom(src => GetHomeTeamName(src)))
            .ForMember(d => d.HomeTeamId, opt => opt.MapFrom(src => GetHomeTeamId(src)))
            .ForMember(d => d.AwayTeamName, opt => opt.MapFrom(src => GetAwayTeamName(src)))
            .ForMember(d => d.AwayTeamId, opt => opt.MapFrom(src => GetAwayTeamId(src)));

            CreateMap<FacilityRating, FacilityRatingDto>();
            CreateMap<FacilityRatingForCreationDto, FacilityRating>()
                .ForMember(dest => dest.Stars, opt => opt.MapFrom(src => src.Stars))
                .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment));
            CreateMap<FacilityRatingForUpdateDto, FacilityRating>()
                .ForMember(dest => dest.Stars, opt => opt.MapFrom(src => src.Stars))
                .ForMember(dest => dest.Comment, opt => opt.MapFrom(src => src.Comment));

            CreateMap<WeeklyOpening, WeeklyOpeningDto>().ReverseMap();
            CreateMap<WeeklyOpeningForCreationDto, WeeklyOpening>();

            CreateMap<FieldException, FieldExceptionDto>().ReverseMap();
            CreateMap<FieldExceptionForCreationDto, FieldException>();

            CreateMap<FieldForCreationDto, Field>();
            CreateMap<FieldForUpdateDto, Field>()
                .ForMember(dest => dest.WeeklyOpenings, opt => opt.Ignore())
                .ForMember(dest => dest.Exceptions, opt => opt.Ignore());

            CreateMap<FieldException, FieldExceptionDto>().ReverseMap();
            CreateMap<FieldExceptionForCreationDto, FieldException>();

            CreateMap<MonthlyMembership, MonthlyMembershipDto>();
            CreateMap<MonthlyMembershipForCreationDto, MonthlyMembership>();

            CreateMap<Friendship, FriendshipDto>();

            CreateMap<FacilityRating, FacilityRatingDto>();
            CreateMap<FacilityRatingForCreationDto, FacilityRating>();

            CreateMap<Match, MatchDto>().ReverseMap();
            
            CreateMap<Notification, NotificationDto>().ReverseMap();
            CreateMap<NotificationForCreationDto, Notification>();

            /*
                        CreateMap<Field, FieldDto>()
                        .ForMember(
                            dest => dest.Reservations,
                            opt => opt.MapFrom(src => src.Rooms));
                        CreateMap<Field, FieldDto>()
                        .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => src.Facility.OwnerId));
            */
            CreateMap<Field, FieldDto>()
                .ForMember(dest => dest.OwnerId, opt => opt.MapFrom(src => src.Facility.OwnerId))
                .ForMember(dest => dest.Reservations, opt => opt.MapFrom(src => src.Rooms))
                .ForMember(dest => dest.AvgRating, opt => opt.MapFrom(src => src.AvgRating));
            CreateMap<FieldForCreationDto, Field>();
            CreateMap<FieldForUpdateDto, Field>();

            CreateMap<Photo, PhotoDto>();
            CreateMap<PhotoForCreationDto, Photo>();
        }
    }
}
