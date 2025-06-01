// ------ Ortak baz (okuma DTO’ları için kalıtıma açık) ------------------
namespace Shared.DataTransferObjects;

public record BaseCommentDto(
    int Id,
    int AuthorId,
    string Content,
    int Rating,
    DateTime CreatedAt);

// -----------------------------------------------------------------------
// Saha (Field) ----------------------------------------------------------
// normal mutable DTO sınıfı
public class FieldCommentDto
{
    public int Id { get; set; }
    public int FieldId { get; set; }
    public string Content { get; set; } = null!;
    public int Rating { get; set; }
    public int FromUserId { get; set; }
    public DateTime CreatedAt { get; set; }
}


public record FieldCommentForCreationDto(
    int FieldId,
    string Content,
    int Rating);

public record FieldCommentForUpdateDto(
    string Content,
    int Rating);

// -----------------------------------------------------------------------
// Takım (Team) ----------------------------------------------------------
public record TeamCommentDto(
    int Id,
    int AuthorId,
    int TeamId,
    string Content,
    int Rating,
    DateTime CreatedAt) : BaseCommentDto(Id, AuthorId, Content, Rating, CreatedAt);

public record TeamCommentForCreationDto(
    int AuthorId,
    int TeamId,
    string Content,
    int Rating);

public record TeamCommentForUpdateDto(
    string Content,
    int Rating);

// -----------------------------------------------------------------------
// Kullanıcı (Customer) ---------------------------------------------------
public record UserCommentDto(
    int Id,
    int FromUserId,
    int ToUserId,
    string Content,
    int Rating,
    DateTime CreatedAt) : BaseCommentDto(Id, FromUserId, Content, Rating, CreatedAt);

public record UserCommentForCreationDto(
    int FromUserId,
    int ToUserId,
    string Content,
    int Rating);

public record UserCommentForUpdateDto(
    string Content,
    int Rating);
