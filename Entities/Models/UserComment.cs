using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Models;

/// <summary>
/// Bir müşterinin başka bir müşteriye bıraktığı yorum.
/// </summary>
public class UserComment
{
    public int Id { get; set; }

    /*  --- Yazan kişi --- */
    public int FromUserId { get; set; }

    [ForeignKey(nameof(FromUserId))]
    public Customer? FromUser { get; set; }

    /*  --- Hedef kullanıcı --- */
    public int ToUserId { get; set; }

    [ForeignKey(nameof(ToUserId))]
    public Customer? ToUser { get; set; }

    /*  --- İçerik --- */
    public string Content { get; set; } = string.Empty;

    /// <summary>0–5 arası yıldız puan</summary>
    public int Rating { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }


    /*  --- Opsiyonel soft-delete --- */
    public bool IsDeleted { get; set; } = false;
    public DateTime? DeletedAt { get; set; }
}
