using System;

namespace Contracts;

public interface IRepositoryManager
{
    IAnnouncementRepository Announcement { get; }
    ICommentRepository Comment { get; }
    IEquipmentRepository Equipment { get; }
    IFacilityRatingRepository FacilityRating { get; }
    IFacilityRepository Facility { get; }
    IFieldCommentRepository FieldComment { get; }
    IFieldRepository Field { get; }
    IFriendshipRepository Friendship { get; }
    IMatchRepository Match { get; }
    IMatchRequestRepository MatchRequest { get; }
    IMonthlyMembershipRepository MonthlyMembership { get; }
    INotificationRepository Notification { get; }
    IPhotoRepository Photo { get; }
    IReservationParticipantRepository ReservationParticipant { get; }
    IReservationRepository Reservation { get; }
    ITeamCommentRepository TeamComment { get; }
    ITeamJoinRequestRepository TeamJoinRequest { get; }
    IWeeklyOpeningRepository WeeklyOpening { get; }
    IFieldExceptionRepository FieldException { get; }
    ITeamMemberRepository TeamMember { get; }
    ITeamRepository Team { get; }
    IUserCommentRepository UserComment { get; }
    Task SaveAsync();

}
