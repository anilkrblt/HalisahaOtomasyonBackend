using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Repository;

/// <summary>
/// Uygulamanın ana DbContext’i —
/// Identity + halısaha domain tabloları
/// </summary>
public class RepositoryContext
    : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
  public RepositoryContext(DbContextOptions options) : base(options) { }

  /*────────────────────────────── On-Model-Creating ─────────────────────────────*/
  protected override void OnModelCreating(ModelBuilder mb)
  {
    base.OnModelCreating(mb);



    /*─ Facility ─*/
    mb.Entity<Facility>()
      .HasIndex(f => f.Email)
      .IsUnique();

    /*─ ENUM ↔ int Conversions ─*/
    mb.Entity<Notification>().Property(n => n.Type).HasConversion<int>();
    mb.Entity<Room>().Property(r => r.Status).HasConversion<int>();
    mb.Entity<Room>().Property(r => r.AccessType).HasConversion<int>();
    mb.Entity<TeamJoinRequest>().Property(r => r.Status).HasConversion<int>();

    mb.Entity<TeamMember>()
        .HasKey(tm => new { tm.TeamId, tm.UserId });

    mb.Entity<TeamMember>()
        .HasOne(tm => tm.User)
        .WithMany(c => c.TeamMemberships)
        .HasForeignKey(tm => tm.UserId);

    mb.Entity<TeamMember>()
        .HasOne(tm => tm.Team)
        .WithMany(t => t.Members)
        .HasForeignKey(tm => tm.TeamId);




    mb.Entity<Friendship>().HasKey(f => new { f.UserId1, f.UserId2 });


    mb.Entity<RoomParticipant>(entity =>
 {
   // Tekil Primary Key
   entity.HasKey(rp => rp.Id);

   // Aynı oyuncu aynı odada sadece bir kez yer alabilir
   entity.HasIndex(rp => new { rp.RoomId, rp.CustomerId })
        .IsUnique()
        .HasFilter("CustomerId IS NOT NULL"); // null olanlar için conflict olmasın


   // Room ilişkisi
   entity.HasOne(rp => rp.Room)
        .WithMany(r => r.Participants)
        .HasForeignKey(rp => rp.RoomId)
        .OnDelete(DeleteBehavior.Cascade);

   // Team ilişkisi
   entity.HasOne(rp => rp.Team)
        .WithMany(t => t.TeamReservations)
        .HasForeignKey(rp => rp.TeamId)
        .OnDelete(DeleteBehavior.Cascade);

   // Customer ilişkisi (opsiyonel olabilir)
   entity.HasOne(rp => rp.Customer)
        .WithMany(c => c.RoomParticipations)
        .HasForeignKey(rp => rp.CustomerId)
        .OnDelete(DeleteBehavior.NoAction);
 });




    mb.Entity<Reservation>()
        .HasIndex(r => new { r.FieldId, r.SlotStart })
        .IsUnique();

    mb.Entity<Reservation>()
        .HasOne(r => r.Room)
        .WithOne(room => room.Reservation)
        .HasForeignKey<Reservation>(r => r.RoomId)
        .IsRequired(false);


    mb.Entity<FacilityRating>().HasKey(fr => new { fr.FacilityId, fr.UserId });

    /*──────── Room ⇄ Match (1-1) ─────────────────────────────*/
    mb.Entity<Room>()
      .HasOne(r => r.Match)
      .WithOne(m => m.Room)
      .HasForeignKey<Match>(m => m.RoomId)
      .OnDelete(DeleteBehavior.Cascade);

    /*──────── JoinCode benzersiz (sadece Private odalar) ─────*/
    mb.Entity<Room>()
      .HasIndex(r => r.JoinCode)
      .IsUnique()
      .HasFilter("[JoinCode] IS NOT NULL");

    /*──────── TeamJoinRequest unique & FK ────────────────────*/
    mb.Entity<TeamJoinRequest>(b =>
    {
      b.HasIndex(r => new { r.TeamId, r.UserId }).IsUnique();
      b.HasOne(r => r.Team)
           .WithMany(t => t.JoinRequests)
           .HasForeignKey(r => r.TeamId)
           .OnDelete(DeleteBehavior.Cascade);
    });

    /*──────── Friendship iki yönlü ───────────────────────────*/
    mb.Entity<Friendship>(cfg =>
    {
      cfg.HasKey(f => new { f.UserId1, f.UserId2 });

      cfg.HasOne(f => f.User1)
             .WithMany(c => c.Friends1)
             .HasForeignKey(f => f.UserId1)
             .OnDelete(DeleteBehavior.Restrict);

      cfg.HasOne(f => f.User2)
             .WithMany(c => c.Friends2)
             .HasForeignKey(f => f.UserId2)
             .OnDelete(DeleteBehavior.Restrict);

      cfg.HasIndex(f => new { f.UserId1, f.Status });
      cfg.HasIndex(f => new { f.UserId2, f.Status });
    });

    /*──────── Match ⇄ Team (Home / Away) ─────────────────────*/
    mb.Entity<Match>()
      .HasOne(m => m.HomeTeam)
      .WithMany(t => t.HomeMatches)
      .HasForeignKey(m => m.HomeTeamId)
      .OnDelete(DeleteBehavior.NoAction);

    mb.Entity<Match>()
      .HasOne(m => m.AwayTeam)
      .WithMany(t => t.AwayMatches)
      .HasForeignKey(m => m.AwayTeamId)
      .OnDelete(DeleteBehavior.NoAction);


    /*──────── UserComment ↔ Customer ─────────────────────────*/
    mb.Entity<UserComment>(b =>
    {
      b.HasOne(uc => uc.FromUser)
           .WithMany(c => c.SentComments)
           .HasForeignKey(uc => uc.FromUserId)
           .OnDelete(DeleteBehavior.NoAction);

      b.HasOne(uc => uc.ToUser)
           .WithMany(c => c.ReceivedComments)
           .HasForeignKey(uc => uc.ToUserId)
           .OnDelete(DeleteBehavior.NoAction);
    });

    /*──────── Field ilişkileri ───────────────────────────────*/

    mb.Entity<WeeklyOpening>()
    .HasIndex(o => new { o.FieldId, o.DayOfWeek })
    .IsUnique();

    mb.Entity<FieldException>()
        .HasIndex(e => new { e.FieldId, e.Date })
        .IsUnique();

    mb.Entity<WeeklyOpening>()
      .HasOne(w => w.Field)
      .WithMany(f => f.WeeklyOpenings)
      .HasForeignKey(w => w.FieldId);

    mb.Entity<FieldException>()
      .HasOne(e => e.Field)
      .WithMany(f => f.Exceptions)
      .HasForeignKey(e => e.FieldId);
  }

  /*────────────────────────────── DbSet’ler ──────────────────────────────*/
  public DbSet<Announcement> Announcements => Set<Announcement>();
  public DbSet<Comment> Comments => Set<Comment>();
  public DbSet<FieldComment> FieldComments => Set<FieldComment>();
  public DbSet<TeamComment> TeamComments => Set<TeamComment>();

  public DbSet<Facility> Facilities => Set<Facility>();
  public DbSet<Field> Fields => Set<Field>();
  public DbSet<WeeklyOpening> WeeklyOpenings => Set<WeeklyOpening>();
  public DbSet<FieldException> FieldExceptions => Set<FieldException>();

  public DbSet<Match> Matches => Set<Match>();      // 1-1 Room FK
  public DbSet<MonthlyMembership> MonthlyMemberships => Set<MonthlyMembership>();
  public DbSet<Notification> Notifications => Set<Notification>();

  public DbSet<Room> Rooms => Set<Room>();
  public DbSet<RoomParticipant> RoomParticipants => Set<RoomParticipant>();

  public DbSet<Team> Teams => Set<Team>();
  public DbSet<TeamJoinRequest> TeamJoinRequests => Set<TeamJoinRequest>();
  public DbSet<TeamMember> TeamMembers => Set<TeamMember>();

  public DbSet<UserComment> UserComments => Set<UserComment>();
  public DbSet<Equipment> Equipments => Set<Equipment>();
  public DbSet<Photo> Photos => Set<Photo>();
  public DbSet<FacilityRating> FacilityRatings => Set<FacilityRating>();
  public DbSet<Friendship> Friendships => Set<Friendship>();
  public DbSet<Reservation> Reservations => Set<Reservation>();
  public DbSet<ReservationPayment> ReservationPayments => Set<ReservationPayment>();


}
