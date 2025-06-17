namespace Entities.Models;

public class FieldComment
{
    public int Id { get; set; }
    public int FromUserId { get; set; }
    public Customer? FromUser { get; set; }
    public int FieldId { get; set; }
    public Field? Field { get; set; }
    public string Content { get; set; } = string.Empty;
    public int Rating { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
}