namespace Entities.Models
{
    public enum FloorType
    {
        Dogal = 0,   // Doğal çim
        Sentetik = 1,   // Sentetik çim
        Parke = 2    // Kapalı parke zemin
    }
    public class Field
    {
        public int Id { get; set; }
        public int FacilityId { get; set; }
        public Facility Facility { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Name { get; set; }
        public bool IsIndoor { get; set; }
        public bool IsAvailable { get; set; } = true;
       
        public bool HasCamera { get; set; }
        public FloorType FloorType { get; set; }
        public int Capacity { get; set; }
        public decimal PricePerHour { get; set; }
        public bool LightingAvailable { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<Reservation> Reservations { get; set; }
        public ICollection<WeeklyOpening> WeeklyOpenings { get; set; } = [];
        public ICollection<FieldException> Exceptions { get; set; } = [];

        public ICollection<MonthlyMembership> MonthlyMemberships { get; set; }
    }
}
