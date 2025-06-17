using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service;

public class CommentService : ICommentService
{
    private readonly IRepositoryManager _repo;
    private readonly IMapper _mapper;
    private readonly UserManager<ApplicationUser> _userManager;


    public CommentService(IRepositoryManager repo,
                          IMapper mapper,
                          UserManager<ApplicationUser> userManager)
    {
        _repo = repo;
        _mapper = mapper;
        _userManager = userManager;
    }

    public async Task<FieldCommentDto> GetFieldCommentAsync(int commentId, bool trackChanges)
    {
        var comment = await _repo.FieldComment.GetFieldCommentAsync(commentId, trackChanges);
        if (comment is null)
            throw new CommentNotFoundException(commentId);

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

        var entity = _mapper.Map<FieldComment>(dto);
        entity.FromUserId = fromUserId;

        _repo.FieldComment.CreateFieldComment(entity);
        await _repo.SaveAsync();

        return _mapper.Map<FieldCommentDto>(entity);
    }

    public async Task<FieldCommentDto> UpdateFieldCommentAsync(int commentId, FieldCommentForUpdateDto dto)
    {
        var entity = await _repo.FieldComment.GetFieldCommentAsync(commentId, trackChanges: true) ?? throw new CommentNotFoundException(commentId);

        _mapper.Map(dto, entity);

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
        var entity = await _repo.TeamComment
            .GetTeamCommentAsync(commentId, true)
            ?? throw new CommentNotFoundException(commentId);

        _mapper.Map(dto, entity);

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
        var exists = await _userManager.Users.AnyAsync(u => u.Id == dto.ToUserId);

        if(exists is false)
            throw new UserNotFoundException(dto.ToUserId);

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
        var entity = await _repo.UserComment
            .GetUserCommentAsync(commentId, true)
            ?? throw new CommentNotFoundException(commentId);

        _mapper.Map(dto, entity);

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
