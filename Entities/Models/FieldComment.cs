namespace Entities.Models;

/// <summary>
/// Bir sahaya (Field) bırakılan puanlı yorum.
/// </summary>
public class FieldComment
{
    public int Id { get; set; }

    /* —— Yazan Oyuncu —— */
    public int FromUserId { get; set; }
    public Customer? FromUser { get; set; }

    /* —— Hedef Saha —— */
    public int FieldId { get; set; }
    public Field? Field { get; set; }

    /* —— İçerik —— */
    public string Content { get; set; } = string.Empty;

    /// <summary>1 – 5 arası yıldız puanı.</summary>
    public int Rating { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }


    /* —— Opsiyonel soft-delete —— */
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
}

