using Shared.DataTransferObjects;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Service.Contracts;

/// <summary>
/// Saha, takım ve kullanıcı yorumlarını yöneten tek servis.
/// </summary>
public interface ICommentService
{
    /* ========== FIELD COMMENTS ========== */



    Task<FieldCommentDto> GetFieldCommentAsync(int commentId, bool trackChanges);
    Task<IEnumerable<FieldCommentDto>> GetFieldCommentsAsync(int fieldId, bool trackChanges);
    Task<FieldCommentDto> AddFieldCommentAsync(FieldCommentForCreationDto dto, int fromUserId);
    Task<FieldCommentDto> UpdateFieldCommentAsync(int commentId, FieldCommentForUpdateDto dto);
    Task DeleteFieldCommentAsync(int commentId);




    Task<IEnumerable<TeamCommentDto>> GetTeamCommentsAsync(int teamId, bool trackChanges);
    Task<TeamCommentDto> GetTeamCommentAsync(int commentId, bool trackChanges);
    Task<TeamCommentDto> AddTeamCommentAsync(TeamCommentForCreationDto dto, int fromUserId);
    Task<TeamCommentDto> UpdateTeamCommentAsync(int commentId, TeamCommentForUpdateDto dto);
    Task DeleteTeamCommentAsync(int commentId);

    // User
    Task<IEnumerable<UserCommentDto>> GetCommentsAboutUserAsync(int toUserId, bool trackChanges);
    Task<IEnumerable<UserCommentDto>> GetCommentsFromUserAsync(int fromUserId, bool trackChanges);
    Task<UserCommentDto> GetUserCommentAsync(int commentId, bool trackChanges);
    Task<UserCommentDto> AddUserCommentAsync(UserCommentForCreationDto dto, int fromUserId);
    Task<UserCommentDto> UpdateUserCommentAsync(int commentId, UserCommentForUpdateDto dto);
    Task DeleteUserCommentAsync(int commentId);
}
