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
    private readonly IUserValidationService _userValidationService;

    public CommentService(IRepositoryManager repo,
                          IMapper mapper,
                          IUserValidationService userValidationService)
    {
        _repo = repo;
        _mapper = mapper;
        _userValidationService = userValidationService;
    }

    public async Task<FieldCommentDto> GetFieldCommentAsync(int commentId, bool trackChanges)
    {
        var entity = await CheckFieldCommentExists(commentId);

        return _mapper.Map<FieldCommentDto>(entity);
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

        var entity = _mapper.Map<FieldComment>(dto);
        entity.FromUserId = fromUserId;

        _repo.FieldComment.CreateFieldComment(entity);
        await _repo.SaveAsync();

        return _mapper.Map<FieldCommentDto>(entity);
    }

    public async Task<FieldCommentDto> UpdateFieldCommentAsync(int commentId, FieldCommentForUpdateDto dto)
    {
        var entity = await CheckFieldCommentExists(commentId);

        _mapper.Map(dto, entity);

        await _repo.SaveAsync();
        return _mapper.Map<FieldCommentDto>(entity);
    }

    public async Task DeleteFieldCommentAsync(int commentId)
    {
        var entity = await CheckFieldCommentExists(commentId);
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
        var entity = await CheckTeamCommentExists(commentId);

        return _mapper.Map<TeamCommentDto>(entity);
    }

    public async Task<TeamCommentDto> AddTeamCommentAsync(
        TeamCommentForCreationDto dto,
        int fromUserId)
    {
        _ = await _repo.Team.GetTeamAsync(dto.ToTeamId, false)
            ?? throw new TeamNotFoundException(dto.ToTeamId);

        var teamComment = _mapper.Map<TeamComment>(dto);
        teamComment.FromUserId = fromUserId;

        _repo.TeamComment.CreateTeamComment(teamComment);
        await _repo.SaveAsync();

        return _mapper.Map<TeamCommentDto>(teamComment);
    }

    public async Task<TeamCommentDto> UpdateTeamCommentAsync(
        int commentId,
        TeamCommentForUpdateDto dto)
    {
        var entity = await CheckTeamCommentExists(commentId);

        _mapper.Map(dto, entity);

        await _repo.SaveAsync();
        return _mapper.Map<TeamCommentDto>(entity);
    }

    public async Task DeleteTeamCommentAsync(int commentId)
    {
        var entity = await CheckTeamCommentExists(commentId);

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        await _repo.SaveAsync();
    }

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
        var entity = await CheckUserCommentExists(commentId);

        return _mapper.Map<UserCommentDto>(entity);
    }

    public async Task<UserCommentDto> AddUserCommentAsync(
        UserCommentForCreationDto dto,
        int fromUserId)
    {
        await _userValidationService.CheckUserExists(dto.ToUserId);

        var entity = _mapper.Map<UserComment>(dto);
        entity.FromUserId = fromUserId;

        _repo.UserComment.CreateUserComment(entity);
        await _repo.SaveAsync();

        return _mapper.Map<UserCommentDto>(entity);
    }

    public async Task<UserCommentDto> UpdateUserCommentAsync(
        int commentId,
        UserCommentForUpdateDto dto)
    {
        var entity = await CheckUserCommentExists(commentId);

        _mapper.Map(dto, entity);

        await _repo.SaveAsync();
        return _mapper.Map<UserCommentDto>(entity);
    }

    public async Task DeleteUserCommentAsync(int commentId)
    {
        var entity = await CheckUserCommentExists(commentId);

        entity.IsDeleted = true;
        entity.DeletedAt = DateTime.UtcNow;
        await _repo.SaveAsync();
    }

    private async Task<UserComment> CheckUserCommentExists(int commentId)
    {
        var comment = await _repo.UserComment
            .GetUserCommentAsync(commentId, true);

        if (comment is null)
            throw new CommentNotFoundException(commentId);

        return comment;
    }

    private async Task<TeamComment> CheckTeamCommentExists(int commentId)
    {
        var comment = await _repo.TeamComment
            .GetTeamCommentAsync(commentId, true);

        if (comment is null)
            throw new CommentNotFoundException(commentId);

        return comment;
    }

    private async Task<FieldComment> CheckFieldCommentExists(int commentId)
    {
        var comment = await _repo.FieldComment
            .GetFieldCommentAsync(commentId, true);

        if (comment is null)
            throw new CommentNotFoundException(commentId);

        return comment;
    }
}
