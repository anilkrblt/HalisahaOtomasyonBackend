using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Contracts
{
    public interface IServiceManager
    {

        IAnnouncementService AnnouncementService { get; }
        IAuthService AuthService { get; }
        ICommentService CommentService { get; }
        IEquipmentService EquipmentService { get; }
        IFacilityRatingService FacilityRatingService { get; }
        IFacilityService FacilityService { get; }
        IFieldService FieldService { get; }
        IFriendshipService FriendshipService { get; }
        IMatchService MatchService { get; }
        INotificationService NotificationService { get; }
        IPhotoService PhotoService { get; }
        IRoomService RoomService { get; }
        ITeamService TeamService { get; }
        IReservationService ReservationService { get; }
    }
}
