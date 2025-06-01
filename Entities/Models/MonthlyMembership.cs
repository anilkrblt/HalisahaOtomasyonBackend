namespace Entities.Models;

/// <summary>
/// Sahaya ait aylık üyelik paketi (sınırsız kullanım vb.).
/// </summary>
public class MonthlyMembership
{
    public int Id { get; set; }

    /* —— Hedef saha —— */
    public int FieldId  { get; set; }          // düzeltildi
    public Field? Field { get; set; }

    /* —— Üye (Customer) —— */
    public int UserId   { get; set; }
    public Customer? User { get; set; }

    /* —— Tarihler —— */
    public DateTime CreatedAt      { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt      { get; set; } = DateTime.UtcNow;
    public DateTime ExpirationDate { get; set; }

    /* —— Opsiyonel soft-delete —— */
    // public bool IsCancelled  { get; set; }
    // public DateTime? CancelledAt { get; set; }
}
