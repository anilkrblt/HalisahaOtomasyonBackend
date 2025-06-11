
// kullanıcıların facility yorumlarını getir
// user rating atması için yeni bir tablo ratings tablosu yazılacak
// user id ile ilişkili tüm fcilityler dönsün


using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models
{

    public class Facility
    {
        public int Id { get; set; }

        [MaxLength(128)]
        public string Name { get; set; } = null!;
        public string? LogoUrl { get; set; }

        /* Sahibi */
        public int OwnerId { get; set; }
        public Owner? Owner { get; set; }

        [Column(TypeName = "decimal(18,15)")]
        public decimal? Latitude { get; set; }

        [Column(TypeName = "decimal(18,15)")]
        public decimal? Longitude { get; set; }


        /* Adres */
        public string? City { get; set; }
        public string? Town { get; set; }
        public string? AddressDetails { get; set; }
        public string? Description { get; set; }

        /* Özellikler */
        public bool HasLockableCabinet { get; set; }
        public bool HasCafeteria { get; set; }
        // yeni
        public bool HasLockerRoom { get; set; }
        public bool HasFirstAid { get; set; }
        public bool HasSecurityCameras { get; set; }
        public bool HasRefereeService { get; set; }
        //
        public bool HasParking { get; set; }

        public bool HasShower { get; set; }
        public bool HasToilet { get; set; }
        public bool HasTransportService { get; set; }

        /* İletişim */
        [EmailAddress]
        public string? Email { get; set; }
        [MaxLength(32)]
        public string Phone { get; set; } = string.Empty;

        /* Finans */
        public string? BankAccountInfo { get; set; }

        /* Rating (0-5) */
        public double Rating { get; set; }
        public ICollection<FacilityRating> Ratings { get; set; } = [];

        /* Navigation */
        public ICollection<Photo> Photos { get; set; } = new List<Photo>();
        public ICollection<Equipment> Equipments { get; set; } = [];
        public ICollection<Field> Fields { get; set; } = [];
        public ICollection<FieldComment> Comments { get; set; } = [];
        public ICollection<Announcement> Announcements { get; set; } = [];
        public ICollection<Match> Matches { get; set; } = [];
        public ICollection<Notification> Notifications { get; set; } = [];
    }

}
