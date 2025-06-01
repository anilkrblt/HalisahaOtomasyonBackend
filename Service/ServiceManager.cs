using AutoMapper;
using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Service.Contracts;
using Service.Hubs;
using StackExchange.Redis;
using System;

namespace Service;

public class ServiceManager : IServiceManager
{
    /*───────── Lazy alanlar ─────────*/
    private readonly Lazy<IPhotoService> _photo;

    private readonly Lazy<IAnnouncementService> _announcement;
    private readonly Lazy<INotificationService> _notification;
    private readonly Lazy<IAuthService> _auth;
    private readonly Lazy<ICommentService> _comment;
    private readonly Lazy<IEquipmentService> _equipment;
    private readonly Lazy<IFacilityRatingService> _facilityRating;
    private readonly Lazy<IFacilityService> _facility;
    private readonly Lazy<IFieldService> _field;
    private readonly Lazy<IFriendshipService> _friendship;
    private readonly Lazy<IMatchService> _match;
    private readonly Lazy<IReservationService> _reservation;
    private readonly Lazy<ITeamService> _team;

    public ServiceManager(
        /* ①  User türü => ApplicationUser (üst sınıf) */
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole<int>> roleManager,
        IConfiguration config,
        IConnectionMultiplexer redis,
        IRepositoryManager repo,
        ILoggerManager log,
        IMapper map,
        IHubContext<NotificationHub> hub)
    {
        /* Auth */
        _auth = new(() =>
            new AuthService(userManager, signInManager, config, roleManager, redis));
        _photo = new(() => new PhotoService(repo, map));
        _facility = new(() => new FacilityService(repo, log, map, _photo.Value));
        _notification = new(() => new NotificationService(repo, log, map, hub));

        /* Core servisler */
        _comment = new(() => new CommentService(repo, map, log));
        _field = new(() => new FieldService(repo, log, map, _photo.Value));
        _reservation = new(() => new ReservationService(repo, _notification.Value, map));
        _equipment = new(() => new EquipmentService(repo, map));
        _announcement = new(() => new AnnouncementService(repo, log, map));
        _match = new(() => new MatchService(repo, map));

        /* Yeni servisler */
        _facilityRating = new(() => new FacilityRatingService(repo, map));
        _friendship = new(() => new FriendshipService(repo, map));
        _team = new(() => new TeamService(repo, map, log, _photo.Value));
    }

    /*───────── Arabirim üyeleri ─────────*/
    public IPhotoService PhotoService => _photo.Value;

    public IAnnouncementService AnnouncementService => _announcement.Value;
    public IAuthService AuthService => _auth.Value;
    public ICommentService CommentService => _comment.Value;
    public IEquipmentService EquipmentService => _equipment.Value;
    public IFacilityRatingService FacilityRatingService => _facilityRating.Value;
    public IFacilityService FacilityService => _facility.Value;
    public IFieldService FieldService => _field.Value;
    public IFriendshipService FriendshipService => _friendship.Value;
    public IMatchService MatchService => _match.Value;
    public INotificationService NotificationService => _notification.Value;
    public IReservationService ReservationService => _reservation.Value;
    public ITeamService TeamService => _team.Value;
}
