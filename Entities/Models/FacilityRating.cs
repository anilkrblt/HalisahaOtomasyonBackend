namespace Entities.Models;

public class FacilityRating
{
    public int FacilityId { get; set; }
    public Facility Facility { get; set; } = null!;

    public int UserId { get; set; }
    public Customer User { get; set; } = null!;

    public int     Stars      { get; set; } // 0-5
    public string? Comment    { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
