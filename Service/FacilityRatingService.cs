// Service/FacilityRatingService.cs
using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service;

// Service/FacilityRatingService.cs

public class FacilityRatingService : IFacilityRatingService
{
    private readonly IRepositoryManager _repo;
    private readonly IMapper _map;

    public FacilityRatingService(IRepositoryManager repo, IMapper map)
    {
        _repo = repo;
        _map = map;
    }

    public async Task<IEnumerable<FacilityRatingDto>> GetRatingsByFacilityAsync(int facilityId, bool trackChanges)
    {
        if (await _repo.Facility.GetFacilityAsync(facilityId, false) is null)
            throw new FacilityNotFoundException(facilityId);

        var ratings = await _repo.FacilityRating
                          .GetRatingsByFacilityIdAsync(facilityId, trackChanges);
        return _map.Map<IEnumerable<FacilityRatingDto>>(ratings);
    }

    public async Task<double> GetAverageStarsAsync(int facilityId)
    {
        var ratings = await _repo.FacilityRating
                          .GetRatingsByFacilityIdAsync(facilityId, false);
        return ratings.Any() ? ratings.Average(r => r.Stars) : 0.0;
    }

    public async Task<FacilityRatingDto> AddRatingAsync(int facilityId, FacilityRatingForCreationDto dto, int userId)
    {
        var facility = await _repo.Facility.GetFacilityAsync(facilityId, true)
                       ?? throw new FacilityNotFoundException(facilityId);

        var existing = await _repo.FacilityRating
                         .GetRatingAsync(facilityId, userId, false);
        if (existing != null)
            throw new InvalidOperationException("You have already rated this facility.");

        var entity = new FacilityRating
        {
            FacilityId = facilityId,
            UserId = userId,
            Stars = dto.Stars,
            Comment = dto.Comment
        };
        _repo.FacilityRating.CreateRating(entity);
        await _repo.SaveAsync();

        // ortalamayı güncelle
        facility.Rating = await GetAverageStarsAsync(facilityId);
        await _repo.SaveAsync();

        return _map.Map<FacilityRatingDto>(entity);
    }

    public async Task UpdateRatingAsync(int facilityId, int userId, FacilityRatingForUpdateDto dto)
    {
        var entity = await _repo.FacilityRating
                         .GetRatingAsync(facilityId, userId, true)
                     ?? throw new RatingNotFoundException(facilityId, userId);

        entity.Stars = dto.Stars;
        entity.Comment = dto.Comment;
        await _repo.SaveAsync();

        // ortalamayı güncelle
        var facility = await _repo.Facility.GetFacilityAsync(facilityId, true)!;
        facility.Rating = await GetAverageStarsAsync(facilityId);
        await _repo.SaveAsync();
    }

    public async Task<IEnumerable<FacilityDto>> GetRatedFacilitiesByUserAsync(int userId, bool trackChanges)
    {
        var ratings = await _repo.FacilityRating.GetRatingsByUserIdAsync(userId, false);
        var facilityIds = ratings.Select(r => r.FacilityId).Distinct();
        var facs = new List<Facility>();
        foreach (var id in facilityIds)
        {
            var f = await _repo.Facility.GetFacilityAsync(id, trackChanges);
            if (f != null) facs.Add(f);
        }
        return _map.Map<IEnumerable<FacilityDto>>(facs);
    }
}

/* Basit istisna */
public sealed class RatingNotFoundException : NotFoundException
{
    public RatingNotFoundException(int facId, int userId)
        : base($"Rating not found (Facility:{facId}, User:{userId}).") { }
}
