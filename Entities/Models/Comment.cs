using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

/// <summary>
/// Her türlü (saha, takım, kullanıcı) yorumu tek tabloda tutar.
/// </summary>
public enum CommentTargetType
{
    Field = 0,
    Team  = 1,
    User  = 2
}

public class Comment
{
    public int Id { get; set; }

    /* —— Hedef —— */
    public CommentTargetType TargetType { get; set; }   // Field / Team / User
    public int TargetId   { get; set; }                 // FieldId | TeamId | ToUserId

    /* —— Yazarı —— */
    public int AuthorId   { get; set; }                 // Customer.Id

    [ForeignKey(nameof(AuthorId))]
    public Customer? Author { get; set; }

    /* —— İçerik —— */
    public string  Content   { get; set; } = string.Empty;
    public int     Rating    { get; set; }              // 1-5 (Field & Team’de kullan)
    public double  Sentiment { get; set; }              // Opsiyonel NLP skoru

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    /* Soft-delete */
    public bool IsDeleted  { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
}
