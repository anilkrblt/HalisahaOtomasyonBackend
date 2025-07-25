using System;
using System.Threading.Tasks;
using Contracts;

namespace Repository;

public class RepositoryManager : IRepositoryManager
{
    private readonly RepositoryContext _ctx;

    private readonly Lazy<IAnnouncementRepository> _announcement;
    private readonly Lazy<ICommentRepository> _comment;
    private readonly Lazy<IEquipmentRepository> _equipment;
    private readonly Lazy<IFacilityRatingRepository> _facilityRating;
    private readonly Lazy<IFacilityRepository> _facility;
    private readonly Lazy<IFieldCommentRepository> _fieldComment;
    private readonly Lazy<IFieldRepository> _field;
    private readonly Lazy<IFriendshipRepository> _friendship;
    private readonly Lazy<IMatchRepository> _match;
    private readonly Lazy<IMonthlyMembershipRepository> _monthlyMembership;
    private readonly Lazy<INotificationRepository> _notification;
    private readonly Lazy<IPhotoRepository> _photo;
    private readonly Lazy<IRoomParticipantRepository> _roomParticipant;
    private readonly Lazy<IRoomRepository> _room;
    private readonly Lazy<ITeamCommentRepository> _teamComment;
    private readonly Lazy<ITeamJoinRequestRepository> _teamJoinRequest;
    private readonly Lazy<ITeamMemberRepository> _teamMember;
    private readonly Lazy<ITeamRepository> _team;
    private readonly Lazy<IUserCommentRepository> _userComment;
    private readonly Lazy<IWeeklyOpeningRepository> _weeklyOpening;
    private readonly Lazy<IFieldExceptionRepository> _fieldException;
    private readonly Lazy<IReservationPaymentRepository> _reservationPayment;
    private readonly Lazy<IReservationRepository> _reservation;
    /* -------- ctor -------- */
    public RepositoryManager(RepositoryContext ctx)
    {
        _ctx = ctx;

        _announcement = new(() => new AnnouncementRepository(ctx));
        _comment = new(() => new CommentRepository(ctx));
        _equipment = new(() => new EquipmentRepository(ctx));
        _facilityRating = new(() => new FacilityRatingRepository(ctx));
        _facility = new(() => new FacilityRepository(ctx));
        _fieldComment = new(() => new FieldCommentRepository(ctx));
        _field = new(() => new FieldRepository(ctx));
        _friendship = new(() => new FriendshipRepository(ctx));
        _match = new(() => new MatchRepository(ctx));
        _monthlyMembership = new(() => new MonthlyMembershipRepository(ctx));
        _notification = new(() => new NotificationRepository(ctx));
        _photo = new(() => new PhotoRepository(ctx));
        _roomParticipant = new(() => new RoomParticipantRepository(ctx));
        _room = new(() => new RoomRepository(ctx));
        _teamComment = new(() => new TeamCommentRepository(ctx));
        _teamJoinRequest = new(() => new TeamJoinRequestRepository(ctx));
        _teamMember = new(() => new TeamMemberRepository(ctx));
        _team = new(() => new TeamRepository(ctx));
        _userComment = new(() => new UserCommentRepository(ctx));
        _weeklyOpening = new(() => new WeeklyOpeningRepository(ctx));
        _fieldException = new(() => new FieldExceptionRepository(ctx));
        _reservationPayment = new(() => new ReservationPaymentRepository(ctx));
        _reservation = new(() => new ReservationRepository(ctx));
    }

    /* -------- Exposed props -------- */
    public IAnnouncementRepository Announcement => _announcement.Value;
    public ICommentRepository Comment => _comment.Value;
    public IEquipmentRepository Equipment => _equipment.Value;
    public IFacilityRatingRepository FacilityRating => _facilityRating.Value;
    public IFacilityRepository Facility => _facility.Value;
    public IFieldCommentRepository FieldComment => _fieldComment.Value;
    public IFieldRepository Field => _field.Value;
    public IFriendshipRepository Friendship => _friendship.Value;
    public IMatchRepository Match => _match.Value;
    public IMonthlyMembershipRepository MonthlyMembership => _monthlyMembership.Value;
    public INotificationRepository Notification => _notification.Value;
    public IPhotoRepository Photo => _photo.Value;
    public IRoomParticipantRepository RoomParticipant => _roomParticipant.Value;
    public IRoomRepository Room => _room.Value;
    public ITeamCommentRepository TeamComment => _teamComment.Value;
    public ITeamJoinRequestRepository TeamJoinRequest => _teamJoinRequest.Value;
    public ITeamMemberRepository TeamMember => _teamMember.Value;
    public ITeamRepository Team => _team.Value;
    public IUserCommentRepository UserComment => _userComment.Value;
    public IWeeklyOpeningRepository WeeklyOpening => _weeklyOpening.Value;
    public IFieldExceptionRepository FieldException => _fieldException.Value;
    public IReservationPaymentRepository ReservationPayment => _reservationPayment.Value;
    public IReservationRepository Reservation => _reservation.Value;


    public async Task SaveAsync() => await _ctx.SaveChangesAsync();
}
