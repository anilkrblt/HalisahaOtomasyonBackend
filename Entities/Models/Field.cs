namespace Entities.Models
{
    public enum FloorType
    {
        Dogal = 0,
        Yapay = 1,
        Parke = 2,
        Kum = 3
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
        public bool HasTribune { get; set; }
        public bool IsAvailable { get; set; } = true;
        public bool HasScoreBoard { get; set; }
        public bool HasCamera { get; set; }
        public FloorType FloorType { get; set; }
        public int Capacity { get; set; }
        public decimal PricePerHour { get; set; }
        public bool LightingAvailable { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<Room> Rooms { get; set; }
        public ICollection<WeeklyOpening> WeeklyOpenings { get; set; } = [];
        public ICollection<FieldException> Exceptions { get; set; } = [];

        public ICollection<MonthlyMembership> MonthlyMemberships { get; set; }
    }
}
