namespace Entities.Models
{
    public class Announcement
    {
        public int Id { get; set; }
        public int FacilityId { get; set; }
        public Facility? Facility { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime EndTime { get; set; }
         
    }
}