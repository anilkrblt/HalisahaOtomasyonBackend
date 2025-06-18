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
    private readonly IPhotoService _photoService;

    public CommentService(IRepositoryManager repo,
                          IMapper mapper,
                          IUserValidationService userValidationService,
                          IPhotoService photoService)
    {
        _repo = repo;
        _mapper = mapper;
        _userValidationService = userValidationService;
        _photoService = photoService;
    }

    public async Task<FieldCommentDto> GetFieldCommentAsync(int commentId, bool trackChanges)
    {
        var entity = await CheckFieldCommentExists(commentId);
        var commentDto = _mapper.Map<FieldCommentDto>(entity);

        var photosDto = await _photoService.GetPhotosAsync("user", commentDto.FromUserId, false);
        commentDto.UserPhotoUrl = photosDto?.FirstOrDefault()?.Url ?? "";

        return commentDto;
    }

    public async Task<IEnumerable<FieldCommentDto>> GetFieldCommentsAsync(int fieldId, bool trackChanges)
    {
        _ = await _repo.Field.GetFieldAsync(fieldId, trackChanges: false) ?? throw new FieldNotFoundException(fieldId);

        var comments = await _repo.FieldComment.GetFieldCommentsByFieldIdAsync(fieldId, trackChanges);
        var commentDtos = _mapper.Map<IEnumerable<FieldCommentDto>>(comments);

        foreach (var commentDto in commentDtos)
        {
            var photosDto = await _photoService.GetPhotosAsync("user", commentDto.FromUserId, false);
            commentDto.UserPhotoUrl = photosDto?.FirstOrDefault()?.Url ?? "";
        }

        return commentDtos;
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

        var teamCommentDtos = _mapper.Map<IEnumerable<TeamCommentDto>>(comments);

        foreach (var teamCommentDto in teamCommentDtos)
        {
            var photosDto = await _photoService.GetPhotosAsync("user", teamCommentDto.FromUserId, false);
            teamCommentDto.UserPhotoUrl = photosDto?.FirstOrDefault()?.Url ?? "";
        }

        return teamCommentDtos;
    }

    public async Task<TeamCommentDto> GetTeamCommentAsync(int commentId, bool trackChanges)
    {
        var entity = await CheckTeamCommentExists(commentId);
        var teamCommentDto = _mapper.Map<TeamCommentDto>(entity);

        var photosDto = await _photoService.GetPhotosAsync("user", teamCommentDto.FromUserId, false);
        teamCommentDto.UserPhotoUrl = photosDto?.FirstOrDefault()?.Url ?? "";

        return teamCommentDto;
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

        var userCommentDtos = _mapper.Map<IEnumerable<UserCommentDto>>(comments);

        foreach (var userCommentDto in userCommentDtos)
        {
            var photosDto = await _photoService.GetPhotosAsync("user", userCommentDto.FromUserId, false);
            userCommentDto.UserPhotoUrl = photosDto?.FirstOrDefault()?.Url ?? "";
        }

        return userCommentDtos;
    }

    public async Task<IEnumerable<UserCommentDto>> GetCommentsFromUserAsync(int fromUserId, bool trackChanges)
    {
        var comments = await _repo.UserComment
            .GetCommentsFromUserAsync(fromUserId, trackChanges);

        var userCommentDtos = _mapper.Map<IEnumerable<UserCommentDto>>(comments);

        foreach (var userCommentDto in userCommentDtos)
        {
            var photosDto = await _photoService.GetPhotosAsync("user", userCommentDto.FromUserId, false);
            userCommentDto.UserPhotoUrl = photosDto?.FirstOrDefault()?.Url ?? "";
        }

        return userCommentDtos;
    }

    public async Task<UserCommentDto> GetUserCommentAsync(int commentId, bool trackChanges)
    {
        var entity = await CheckUserCommentExists(commentId);
        var userCommentDto = _mapper.Map<UserCommentDto>(entity);

        var photosDto = await _photoService.GetPhotosAsync("user", userCommentDto.FromUserId, false);
        userCommentDto.UserPhotoUrl = photosDto?.FirstOrDefault()?.Url ?? "";

        return userCommentDto;
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
