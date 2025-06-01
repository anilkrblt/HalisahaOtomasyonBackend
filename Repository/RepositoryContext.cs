using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class RepositoryContext
    : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
  public RepositoryContext(DbContextOptions options) : base(options) { }

  /*──────────────────── Model Config ────────────────────*/
  protected override void OnModelCreating(ModelBuilder mb)
  {
    base.OnModelCreating(mb);
    // ← Identity tablo/ilişkileri


    /* ---------- Composite Keys ---------- */
    mb.Entity<TeamMember>()
      .HasKey(tm => new { tm.TeamId, tm.UserId });

    mb.Entity<Friendship>()
      .HasKey(f => new { f.UserId1, f.UserId2 });

    mb.Entity<ReservationParticipant>()
      .HasKey(rp => new { rp.ReservationId, rp.TeamId });

    mb.Entity<FacilityRating>()
      .HasKey(fr => new { fr.FacilityId, fr.UserId });

    /* ---------- Enum ↔ int Conversions --- */
    mb.Entity<Notification>()
      .Property(n => n.Type)
      .HasConversion<int>();

    mb.Entity<Reservation>()
      .Property(r => r.Status)
      .HasConversion<int>();

    mb.Entity<ReservationParticipant>()
      .Property(p => p.Status)
      .HasConversion<int>();

    mb.Entity<TeamJoinRequest>()
      .Property(r => r.Status)
      .HasConversion<int>();

    mb.Entity<MatchRequest>()
      .Property(r => r.Status)
      .HasConversion<int>();

    /* Friendship */
    
    mb.Entity<Friendship>(cfg =>
    {
      cfg.HasKey(f => new { f.UserId1, f.UserId2 }); // bileşik PK
      cfg.HasOne(f => f.User1)
         .WithMany(c => c.Friends1)
         .HasForeignKey(f => f.UserId1)
         .OnDelete(DeleteBehavior.Restrict);

      cfg.HasOne(f => f.User2)
         .WithMany(c => c.Friends2)
         .HasForeignKey(f => f.UserId2)
         .OnDelete(DeleteBehavior.Restrict);

      cfg.HasIndex(f => new { f.UserId1, f.Status });      // liste sorguları için
      cfg.HasIndex(f => new { f.UserId2, f.Status });
    });



    mb.Entity<TeamJoinRequest>(b =>
    {
      // Tekrarlayan aynı kullanıcı-takım isteğini engellemek için
      b.HasIndex(r => new { r.TeamId, r.UserId })
   .IsUnique();

      b.HasOne(r => r.Team)
   .WithMany(t => t.JoinRequests)
   .HasForeignKey(r => r.TeamId)
   .OnDelete(DeleteBehavior.Cascade);
    });


    mb.Entity<Match>()
      .HasOne(m => m.HomeTeam)
      .WithMany(t => t.HomeMatches)      // Team.HomeMatches koleksiyonu
      .HasForeignKey(m => m.HomeTeamId)
      .OnDelete(DeleteBehavior.NoAction);   // Cascade / Restrict ihtiyaca göre

    // 2) Deplasman (Away) takım ilişkisi
    mb.Entity<Match>()
      .HasOne(m => m.AwayTeam)
      .WithMany(t => t.AwayMatches)      // Team.AwayMatches koleksiyonu
      .HasForeignKey(m => m.AwayTeamId)
      .OnDelete(DeleteBehavior.NoAction);

    // 2) Yorumu ALAN kullanıcının aldığı yorumlar
    // GÖNDEREN (fromUser) → SentComments
    mb.Entity<UserComment>(b =>
    {
      b.HasOne(uc => uc.FromUser)
   .WithMany(c => c.SentComments)      // ICollection<UserComment>
   .HasForeignKey(uc => uc.FromUserId)
   .OnDelete(DeleteBehavior.NoAction);
    });

    // ALICI (toUser) → ReceivedComments
    mb.Entity<UserComment>(b =>
    {
      b.HasOne(uc => uc.ToUser)
   .WithMany(c => c.ReceivedComments)  // ICollection<UserComment>
   .HasForeignKey(uc => uc.ToUserId)
   .OnDelete(DeleteBehavior.NoAction);
    });
    // RepositoryContext.OnModelCreating
    mb.Entity<Comment>(b =>
    {
      b.HasOne(c => c.Author)
    .WithMany()                     // İstiyorsan Customer'a koleksiyon ekleyip burada kullan
    .HasForeignKey(c => c.AuthorId) // ==> sütun ADI sabitlenmiş olur
    .OnDelete(DeleteBehavior.NoAction);
    });

    mb.Entity<TeamJoinRequest>(b =>
{
  // (a) FK'nin kullanacağı tek-sütun index
  b.HasIndex(r => r.TeamId)
   .HasDatabaseName("IX_TeamJoinRequests_TeamId");

  // (b) 1 takım + 1 user için benzersiz satır
  b.HasIndex(r => new { r.TeamId, r.UserId })
   .IsUnique()
   .HasDatabaseName("UX_TeamJoinRequests_TeamId_UserId");

  // FK
  b.HasOne(r => r.Team)
   .WithMany(t => t.JoinRequests)
   .HasForeignKey(r => r.TeamId)
   .OnDelete(DeleteBehavior.Cascade);
});

    mb.Entity<TeamJoinRequest>(b =>
   {
     // FK'den dolayı TeamId’ye zaten indeks gelecek; bir daha eklemeyin!
     // b.HasIndex(r => r.TeamId);

     // Yalnızca bu benzersiz indeks yeterli
     b.HasIndex(r => new { r.TeamId, r.UserId })
   .IsUnique();                   // --> adı EF’in verdiği varsayılan olsun
   });
    mb.Entity<WeeklyOpening>()
     .HasOne(w => w.Field)
     .WithMany(f => f.WeeklyOpenings)
     .HasForeignKey(w => w.FieldId);

    mb.Entity<FieldException>()
        .HasOne(e => e.Field)
        .WithMany(f => f.Exceptions)
        .HasForeignKey(e => e.FieldId);



  }

  /*──────────────────── DbSet’ler ────────────────────*/
  public DbSet<Announcement> Announcements => Set<Announcement>();
  public DbSet<Comment> Comments => Set<Comment>();
  public DbSet<FieldComment> FieldComments => Set<FieldComment>();
  public DbSet<TeamComment> TeamComments => Set<TeamComment>();
  public DbSet<Facility> Facilities => Set<Facility>();
  public DbSet<Field> Fields => Set<Field>();

  public DbSet<WeeklyOpening> WeeklyOpenings => Set<WeeklyOpening>();
  public DbSet<FieldException> FieldExceptions => Set<FieldException>();
  public DbSet<Match> Matches => Set<Match>();
  public DbSet<MatchRequest> MatchRequests => Set<MatchRequest>();
  public DbSet<MonthlyMembership> MonthlyMemberships => Set<MonthlyMembership>();
  public DbSet<Notification> Notifications => Set<Notification>();
  public DbSet<Reservation> Reservations => Set<Reservation>();
  public DbSet<ReservationParticipant> ReservationParticipants => Set<ReservationParticipant>();
  public DbSet<Team> Teams => Set<Team>();
  public DbSet<TeamJoinRequest> TeamJoinRequests => Set<TeamJoinRequest>();
  public DbSet<TeamMember> TeamMembers => Set<TeamMember>();
  public DbSet<UserComment> UserComments => Set<UserComment>();
  public DbSet<Equipment> Equipments => Set<Equipment>();
  public DbSet<Photo> Photos => Set<Photo>();
  public DbSet<FacilityRating> FacilityRatings => Set<FacilityRating>();
  public DbSet<Friendship> Friendships => Set<Friendship>();
}
