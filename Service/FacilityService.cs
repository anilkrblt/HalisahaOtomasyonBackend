using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service;

public class FacilityService : IFacilityService
{
    private readonly IRepositoryManager _repo;
    private readonly IMapper _map;
    private readonly IPhotoService _photo;
    private readonly IUserValidationService _userValidationService;

    public FacilityService(IRepositoryManager repo,
                           IMapper map,
                           IPhotoService photoService,
                           IUserValidationService userValidationService)
    {
        _repo = repo;
        _map = map;
        _photo = photoService;
        _userValidationService = userValidationService;
    }

    public async Task<IEnumerable<FacilityDto>> GetAllFacilitiesAsync(int? ownerId, bool trackChanges)
    {
        IEnumerable<Facility> facilities;

        if (ownerId is null)
            facilities = await _repo.Facility.GetAllFacilitiesAsync(trackChanges);
        else
        {
            await _userValidationService.CheckUserExists(ownerId.Value);
            facilities = await _repo.Facility.GetFacilitiesByOwnerIdAsync(ownerId.Value, false);
        }
        var facilityDtos = _map.Map<IEnumerable<FacilityDto>>(facilities);

        foreach (var facilityDto in facilityDtos)
        {
            var photoDtos = await _photo.GetPhotosAsync("facility", facilityDto.Id, false);
            facilityDto.PhotoUrls = photoDtos.Select(p => p.Url).ToList();
        }

        return facilityDtos;
    }

    public async Task<FacilityDto> GetFacilityAsync(int facilityId, bool track)
    {
        var facility = await CheckFacilityExists(facilityId, track);
        var facilityDto = _map.Map<FacilityDto>(facility);

        var photoDtos = await _photo.GetPhotosAsync("facility", facility.Id, false);
        var photoUrls = photoDtos.Select(p => p.Url).ToList();
        facilityDto.PhotoUrls = photoUrls;

        return facilityDto;
    }

    public async Task<FacilityDto> CreateFacilityAsync(FacilityForCreationDto dto)
    {
        var entity = _map.Map<Facility>(dto);
        _repo.Facility.CreateFacility(entity);
        await _repo.SaveAsync();

        if (dto.LogoFile is not null)
            entity.LogoUrl = await _photo.UploadLogoAsync(dto.LogoFile, $"facility/{entity.Id}");

        await _repo.SaveAsync();

        var facilityDto = _map.Map<FacilityDto>(entity);

        if (dto.PhotoFiles is not null && dto.PhotoFiles.Count > 0)
        {
            await _photo.UploadPhotosAsync(dto.PhotoFiles, "facility", facilityDto.Id);
        }

        return facilityDto;
    }

    public async Task UpdateFacilityAsync(int reviewerId, int facilityId, FacilityForUpdateDto dto, bool track)
    {
        var facility = await CheckFacilityExists(facilityId, true);
        CheckAccess(reviewerId, facilityId, facility.OwnerId);

        if (dto.LogoFile is not null)
            facility.LogoUrl = await _photo.UploadLogoAsync(dto.LogoFile, $"facility/{facilityId}");

        _map.Map(dto, facility);

        await _repo.SaveAsync();
    }

    public async Task PatchFacilityAsync(int reviewerId, int facilityId, FacilityPatchDto patch)
    {
        var facility = await CheckFacilityExists(facilityId, true);
        CheckAccess(reviewerId, facilityId, facility.OwnerId);

        if (patch.Name is not null) facility.Name = patch.Name;
        if (patch.Description is not null) facility.Description = patch.Description;
        if (patch.BankAccountInfo is not null) facility.BankAccountInfo = patch.BankAccountInfo;
        if (patch.Latitude is not null) facility.Latitude = patch.Latitude;
        if (patch.Longitude is not null) facility.Longitude = patch.Longitude;
        if (patch.City is not null) facility.City = patch.City;
        if (patch.Town is not null) facility.Town = patch.Town;
        if (patch.AddressDetails is not null) facility.AddressDetails = patch.AddressDetails;
        if (patch.HasCafeteria is not null) facility.HasCafeteria = patch.HasCafeteria.Value;
        if (patch.HasShower is not null) facility.HasShower = patch.HasShower.Value;
        if (patch.HasLockableCabinet is not null) facility.HasLockableCabinet = patch.HasLockableCabinet.Value;
        if (patch.HasToilet is not null) facility.HasToilet = patch.HasToilet.Value;
        if (patch.HasTransportService is not null) facility.HasTransportService = patch.HasTransportService.Value;
        if (patch.HasParking is not null) facility.HasParking = patch.HasParking.Value;
        if (patch.HasFirstAid is not null) facility.HasFirstAid = patch.HasFirstAid.Value;
        if (patch.HasLockerRoom is not null) facility.HasLockerRoom = patch.HasLockerRoom.Value;
        if (patch.HasSecurityCameras is not null) facility.HasSecurityCameras = patch.HasSecurityCameras.Value;
        if (patch.HasRefereeService is not null) facility.HasRefereeService = patch.HasRefereeService.Value;
        if (patch.Email is not null) facility.Email = patch.Email;
        if (patch.Phone is not null) facility.Phone = patch.Phone;

        await _repo.SaveAsync();
    }

    public async Task UpdateFacilityPhotos(int reviewerId, int facilityId, FacilityPhotosUpdateDto dto)
    {
        var facility = await CheckFacilityExists(facilityId, false);
        CheckAccess(reviewerId, facilityId, facility.OwnerId);

        await _photo.DeletePhotosByEntityAsync("facility", facilityId, trackChanges: true);
        await _photo.UploadPhotosAsync(dto.PhotoFiles, "facility", facilityId);
    }

    public async Task DeleteFacility(int reviewerId, int facilityId, bool track)
    {
        var facility = await CheckFacilityExists(facilityId, false);
        CheckAccess(reviewerId, facilityId, facility.OwnerId);

        await _photo.DeletePhotosByEntityAsync("facility", facilityId, true);

        _repo.Facility.DeleteFacility(facility);
        await _repo.SaveAsync();
    }

    private async Task<Facility> CheckFacilityExists(int facilityId, bool trackChanges)
    {
        var facility = await _repo.Facility.GetFacilityAsync(facilityId, trackChanges);

        if(facility is null)
            throw new FacilityNotFoundException(facilityId);

        return facility;
    }

    private void CheckAccess(int reviewerId, int facilityId, int ownerId)
    {
        if (reviewerId != ownerId)
            throw new UnauthorizedAccessException($"{facilityId} id değerine sahip tesis üzerinde işlem yapma yetkiniz yoktur.");
    }
}
