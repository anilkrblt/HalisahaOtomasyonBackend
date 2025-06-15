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


            // Reservation ↔ ReservationDto
            CreateMap<Reservation, ReservationDto>().ReverseMap();

            // ReservationForCreationDto → Reservation
            CreateMap<ReservationForCreationDto, Reservation>();

            // ReservationPayment ↔ ReservationPaymentDto
            CreateMap<ReservationPayment, ReservationPaymentDto>().ReverseMap();

            // ReservationPaymentForCreationDto → ReservationPayment
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


            // Eğer AutoMapper kullanıyorsan:



            CreateMap<FieldForCreationDto, Field>();

            CreateMap<FieldForUpdateDto, Field>()
                .ForMember(dest => dest.WeeklyOpenings, opt => opt.Ignore())
                .ForMember(dest => dest.Exceptions, opt => opt.Ignore());



            CreateMap<FieldException, FieldExceptionDto>().ReverseMap();
            CreateMap<FieldExceptionForCreationDto, FieldException>();



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





            CreateMap<MonthlyMembership, MonthlyMembershipDto>();
            CreateMap<MonthlyMembershipForCreationDto, MonthlyMembership>();


            CreateMap<Friendship, FriendshipDto>();


            CreateMap<FacilityRating, FacilityRatingDto>();
            CreateMap<FacilityRatingForCreationDto, FacilityRating>();



            CreateMap<Match, MatchDto>().ReverseMap();





            CreateMap<FieldCommentForCreationDto, FieldComment>();
            CreateMap<FieldComment, FieldCommentDto>();

            CreateMap<TeamCommentForCreationDto, TeamComment>();
            CreateMap<TeamComment, TeamCommentDto>();

            CreateMap<UserCommentForCreationDto, UserComment>();
            CreateMap<UserComment, UserCommentDto>();
            // -----------------------------------------------------


            CreateMap<Notification, NotificationDto>().ReverseMap();
            CreateMap<NotificationForCreationDto, Notification>();


            CreateMap<FacilityForCreationDto, Facility>();
            CreateMap<FacilityForUpdateDto, Facility>();

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
                .ForMember(dest => dest.Reservations, opt => opt.MapFrom(src => src.Rooms));




            CreateMap<FieldForCreationDto, Field>();
            CreateMap<FieldForUpdateDto, Field>();






            CreateMap<Photo, PhotoDto>();
            CreateMap<PhotoForCreationDto, Photo>();



        }
    }
}
