namespace Entities.Models
{
    public class Equipment
    {
        public int Id { get; set; }
        public int FacilityId { get; set; }
        public Facility Facility { get; set; }
        public string Name { get; set; }
        public bool IsRentable { get; set; }
        public decimal? Price { get; set; }
        public int Quantity { get; set; }
        public string? Description { get; set; }
    }
}