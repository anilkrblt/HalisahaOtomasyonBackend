using System.ComponentModel.DataAnnotations;

namespace Shared.DataTransferObjects;

public class CommentForAIAnalysisDto
{
    public string Content { get; set; } = string.Empty;
    public int Rating { get; set; }
}

public abstract class CommentDto
{
    public int Id { get; set; }
    public int Rating { get; set; }
    public string Content { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
    public string UserPhotoUrl { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
}

public abstract class CommentDtoForManipulation
{
    public string Content { get; set; }

    [Required(ErrorMessage = "Rating is a required field.")]
    [Range(1, 5)]
    public int Rating { get; set; }
}

public class FieldCommentDto : CommentDto
{
    public int FromUserId { get; set; }
    public int FieldId { get; set; }
}

public class FieldCommentForCreationDto : CommentDtoForManipulation
{
    [Required(ErrorMessage = "FieldId is a required field.")]
    public int FieldId { get; set; }
}

public class FieldCommentForUpdateDto : CommentDtoForManipulation
{

}

public class TeamCommentDto : CommentDto
{
    public int FromUserId { get; set; }
    public int ToTeamId { get; set; }
}

public class TeamCommentForCreationDto : CommentDtoForManipulation
{
    [Required(ErrorMessage = "ToTeamId is a required field.")]
    public int ToTeamId { get; set; }
}

public class TeamCommentForUpdateDto : CommentDtoForManipulation
{

}

public class UserCommentDto : CommentDto
{
    public int FromUserId { get; set; }
    public int ToUserId { get; set; }
}

public class UserCommentForCreationDto : CommentDtoForManipulation
{
    [Required(ErrorMessage = "ToUserId is a required field.")]
    public int ToUserId { get; set; }
}

public class UserCommentForUpdateDto : CommentDtoForManipulation
{

}