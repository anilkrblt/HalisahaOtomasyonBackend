using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

public class UserComment
{
    public int Id { get; set; }
    public int FromUserId { get; set; }

    [ForeignKey(nameof(FromUserId))]
    public Customer? FromUser { get; set; }
    public int ToUserId { get; set; }

    [ForeignKey(nameof(ToUserId))]
    public Customer? ToUser { get; set; }
    public string Content { get; set; } = string.Empty;
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
}
