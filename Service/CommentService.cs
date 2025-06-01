using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service;

public class CommentService : ICommentService
{
    private readonly IRepositoryManager _repo;
    private readonly IMapper _mapper;
    private readonly ILoggerManager _log;

    public CommentService(IRepositoryManager repo,
                          IMapper mapper,
                          ILoggerManager log)
    {
        _repo = repo;
        _mapper = mapper;
        _log = log;
    }



    /*────────────────── FIELD COMMENT ──────────────────*/


    public async Task<FieldCommentDto> GetFieldCommentAsync(int commentId, bool trackChanges)
    {
        var comment = await _repo.FieldComment.GetFieldCommentAsync(commentId, trackChanges);
        return _mapper.Map<FieldCommentDto>(comment);
    }


    public async Task<IEnumerable<FieldCommentDto>> GetFieldCommentsAsync(int fieldId, bool trackChanges)
    {
        _ = await _repo.Field.GetFieldAsync(fieldId, trackChanges: false) ?? throw new FieldNotFoundException(fieldId);

        var comments = await _repo.FieldComment.GetFieldCommentsByFieldIdAsync(fieldId, trackChanges);

        return _mapper.Map<IEnumerable<FieldCommentDto>>(comments);
    }


    public async Task<FieldCommentDto> AddFieldCommentAsync(FieldCommentForCreationDto dto, int fromUserId)
    {
        _ = await _repo.Field.GetFieldAsync(dto.FieldId, trackChanges: false) ?? throw new FieldNotFoundException(dto.FieldId);

        var entity = new FieldComment
        {
            FieldId = dto.FieldId,
            Content = dto.Content,
            Rating = dto.Rating,
            FromUserId = fromUserId,
            CreatedAt = DateTime.UtcNow
        };

        _repo.FieldComment.CreateFieldComment(entity);
        await _repo.SaveAsync();

        return _mapper.Map<FieldCommentDto>(entity);
    }


    public async Task<FieldCommentDto> UpdateFieldCommentAsync(int commentId, FieldCommentForUpdateDto dto)
    {
        var entity = await _repo.FieldComment.GetFieldCommentAsync(commentId, trackChanges: true) ?? throw new CommentNotFoundException(commentId);

        entity.Content = dto.Content;
        entity.Rating = dto.Rating;
        entity.UpdatedAt = DateTime.UtcNow;

        await _repo.SaveAsync();
        return _mapper.Map<FieldCommentDto>(entity);
    }
    public async Task DeleteFieldCommentAsync(int commentId)
    {
        var entity = await _repo.FieldComment.GetFieldCommentAsync(commentId, trackChanges: true) ?? throw new CommentNotFoundException(commentId);
        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        await _repo.SaveAsync();
    }











    public async Task<IEnumerable<TeamCommentDto>> GetTeamCommentsAsync(int teamId, bool trackChanges)
    {
        _ = await _repo.Team.GetTeamAsync(teamId, false)
            ?? throw new TeamNotFoundException(teamId);

        var comments = await _repo.TeamComment
            .GetTeamCommentsByTeamIdAsync(teamId, trackChanges);

        return _mapper.Map<IEnumerable<TeamCommentDto>>(comments);
    }

    public async Task<TeamCommentDto> GetTeamCommentAsync(int commentId, bool trackChanges)
    {
        var entity = await _repo.TeamComment
            .GetTeamCommentAsync(commentId, trackChanges)
            ?? throw new CommentNotFoundException(commentId);

        return _mapper.Map<TeamCommentDto>(entity);
    }

    public async Task<TeamCommentDto> AddTeamCommentAsync(
        TeamCommentForCreationDto dto,
        int fromUserId)
    {
        _ = await _repo.Team.GetTeamAsync(dto.TeamId, false)
            ?? throw new TeamNotFoundException(dto.TeamId);

        var entity = new TeamComment
        {
            Id = dto.TeamId,
            Content = dto.Content,
            Rating = dto.Rating,
            FromUserId = fromUserId,
            CreatedAt = DateTime.UtcNow
        };

        _repo.TeamComment.CreateTeamComment(entity);
        await _repo.SaveAsync();

        return _mapper.Map<TeamCommentDto>(entity);
    }

    public async Task<TeamCommentDto> UpdateTeamCommentAsync(
        int commentId,
        TeamCommentForUpdateDto dto)
    {
        var entity = await _repo.TeamComment
            .GetTeamCommentAsync(commentId, true)
            ?? throw new CommentNotFoundException(commentId);

        entity.Content = dto.Content;
        entity.Rating = dto.Rating;
        entity.UpdatedAt = DateTime.UtcNow;

        await _repo.SaveAsync();
        return _mapper.Map<TeamCommentDto>(entity);
    }

    public async Task DeleteTeamCommentAsync(int commentId)
    {
        var entity = await _repo.TeamComment
            .GetTeamCommentAsync(commentId, true)
            ?? throw new CommentNotFoundException(commentId);

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        await _repo.SaveAsync();
    }

    /*────────────────── USER COMMENT ───────────────────*/

    public async Task<IEnumerable<UserCommentDto>> GetCommentsAboutUserAsync(int toUserId, bool trackChanges)
    {


        var comments = await _repo.UserComment
            .GetCommentsAboutUserAsync(toUserId, trackChanges);

        return _mapper.Map<IEnumerable<UserCommentDto>>(comments);
    }

    public async Task<IEnumerable<UserCommentDto>> GetCommentsFromUserAsync(int fromUserId, bool trackChanges)
    {
        var comments = await _repo.UserComment
            .GetCommentsFromUserAsync(fromUserId, trackChanges);

        return _mapper.Map<IEnumerable<UserCommentDto>>(comments);
    }

    public async Task<UserCommentDto> GetUserCommentAsync(int commentId, bool trackChanges)
    {
        var entity = await _repo.UserComment
            .GetUserCommentAsync(commentId, trackChanges)
            ?? throw new CommentNotFoundException(commentId);

        return _mapper.Map<UserCommentDto>(entity);
    }

    public async Task<UserCommentDto> AddUserCommentAsync(
        UserCommentForCreationDto dto,
        int fromUserId)
    {


        var entity = new UserComment
        {
            ToUserId = dto.ToUserId,
            Content = dto.Content,
            Rating = dto.Rating,
            FromUserId = fromUserId,
            CreatedAt = DateTime.UtcNow
        };

        _repo.UserComment.CreateUserComment(entity);
        await _repo.SaveAsync();

        return _mapper.Map<UserCommentDto>(entity);
    }

    public async Task<UserCommentDto> UpdateUserCommentAsync(
        int commentId,
        UserCommentForUpdateDto dto)
    {
        var entity = await _repo.UserComment
            .GetUserCommentAsync(commentId, true)
            ?? throw new CommentNotFoundException(commentId);

        entity.Content = dto.Content;
        entity.Rating = dto.Rating;
        entity.UpdatedAt = DateTime.UtcNow;

        await _repo.SaveAsync();
        return _mapper.Map<UserCommentDto>(entity);
    }

    public async Task DeleteUserCommentAsync(int commentId)
    {
        var entity = await _repo.UserComment
            .GetUserCommentAsync(commentId, true)
            ?? throw new CommentNotFoundException(commentId);

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        await _repo.SaveAsync();
    }
}
