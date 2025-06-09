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
    mb.Entity<RoomParticipant>().Property(p => p.Status).HasConversion<int>();
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



    mb.Entity<RoomParticipant>().HasKey(rp => new { rp.RoomId, rp.TeamId });
    mb.Entity<RoomParticipant>().HasKey(rp => new { rp.RoomId, rp.TeamId });

    mb.Entity<RoomParticipant>()
        .HasOne(rp => rp.Room)
        .WithMany(r => r.Participants)
        .HasForeignKey(rp => rp.RoomId);

    mb.Entity<RoomParticipant>()
        .HasOne(rp => rp.Team)
        .WithMany(t => t.TeamReservations)
        .HasForeignKey(rp => rp.TeamId);

    mb.Entity<RoomParticipant>()
        .HasIndex(rp => new { rp.RoomId, rp.IsHome })
        .IsUnique();




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
}
