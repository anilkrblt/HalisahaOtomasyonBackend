namespace Entities.Models;

public enum RequestStatus 
{ 
    Pending = 0, 
    Accepted = 1, 
    Rejected = 2, 
    Cancelled = 3 
}

public class TeamJoinRequest
{
    public int Id { get; set; }
    public int TeamId { get; set; }
    public Team? Team { get; set; }
    public int UserId { get; set; }
    public Customer? User { get; set; }
    public RequestStatus Status { get; set; } = RequestStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RespondedAt { get; set; }
}