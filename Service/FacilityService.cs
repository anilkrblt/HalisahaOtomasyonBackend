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
    private readonly ILoggerManager _log;
    private readonly IMapper _map;
    private readonly IPhotoService _photo;

    public FacilityService(IRepositoryManager repo,
                           ILoggerManager log,
                           IMapper map,
                           IPhotoService photoService)
    {
        _repo = repo;
        _log = log;
        _map = map;
        _photo = photoService;
    }

    public async Task<FacilityDto> CreateFacilityAsync(FacilityForCreationDto dto)
    {
        var entity = _map.Map<Facility>(dto);
        _repo.Facility.CreateFacility(entity);
        await _repo.SaveAsync();

        if (dto.LogoFile is not null)
            entity.LogoUrl = await _photo.UploadLogoAsync(dto.LogoFile, $"facility/{entity.Id}");

        await _repo.SaveAsync();
        return _map.Map<FacilityDto>(entity);
    }

    public async Task<IEnumerable<FacilityDto>> GetAllFacilitiesAsync(bool track)
    {
        return _map.Map<IEnumerable<FacilityDto>>(await _repo.Facility.GetAllFacilitiesAsync(track));

    }

    public async Task<FacilityDto> GetFacilityAsync(int id, bool track)
    {
        return _map.Map<FacilityDto>(await _repo.Facility.GetFacilityAsync(id, track) ?? throw new FacilityNotFoundException(id));
    }


    public async Task<IEnumerable<FacilityDto>> GetFacilitiesByOwnerIdAsync(int ownerId, bool track)
    {
        var list = await _repo.Facility.GetAllFacilitiesAsync(track);
        return _map.Map<IEnumerable<FacilityDto>>(list.Where(f => f.OwnerId == ownerId));
    }

    public async Task UpdateFacilityAsync(int id, FacilityForUpdateDto dto, bool track)
    {
        var fac = await _repo.Facility.GetFacilityAsync(id, track) ?? throw new FacilityNotFoundException(id);

        _map.Map(dto, fac);

        if (dto.LogoFile is not null)
            fac.LogoUrl = await _photo.UploadLogoAsync(dto.LogoFile, $"facility/{id}");

        await _repo.SaveAsync();
    }

    public async Task PatchFacilityAsync(int id, FacilityPatchDto patch)
    {
        var fac = await _repo.Facility.GetFacilityAsync(id, true) ?? throw new FacilityNotFoundException(id);

        if (patch.Name is not null) fac.Name = patch.Name;
        if (patch.Description is not null) fac.Description = patch.Description;
        if (patch.BankAccountInfo is not null) fac.BankAccountInfo = patch.BankAccountInfo;
        if (patch.Latitude is not null) fac.Latitude = patch.Latitude;
        if (patch.Longitude is not null) fac.Longitude = patch.Longitude;
        if (patch.City is not null) fac.City = patch.City;
        if (patch.Town is not null) fac.Town = patch.Town;
        if (patch.AddressDetails is not null) fac.AddressDetails = patch.AddressDetails;
        if (patch.HasCafeteria is not null) fac.HasCafeteria = patch.HasCafeteria.Value;
        if (patch.HasShower is not null) fac.HasShower = patch.HasShower.Value;
        if (patch.HasLockableCabinet is not null) fac.HasLockableCabinet = patch.HasLockableCabinet.Value;
        if (patch.HasToilet is not null) fac.HasToilet = patch.HasToilet.Value;
        if (patch.HasTransportService is not null) fac.HasTransportService = patch.HasTransportService.Value;
        if (patch.HasParking is not null) fac.HasParking = patch.HasParking.Value;
        if (patch.HasFirstAid is not null) fac.HasParking = patch.HasFirstAid.Value;
        if (patch.HasLockerRoom is not null) fac.HasParking = patch.HasLockerRoom.Value;
        if (patch.HasSecurityCameras is not null) fac.HasParking = patch.HasSecurityCameras.Value;
        if (patch.HasRefereeService is not null) fac.HasParking = patch.HasRefereeService.Value;
        if (patch.Email is not null) fac.Email = patch.Email;
        if (patch.Phone is not null) fac.Phone = patch.Phone;

        await _repo.SaveAsync();
    }

    public async Task DeleteFacility(int id, bool track)
    {
        var fac = await _repo.Facility.GetFacilityAsync(id, track) ?? throw new FacilityNotFoundException(id);

        await _photo.DeletePhotosByEntityAsync("facility", id, false);

        _repo.Facility.DeleteFacility(fac);
        await _repo.SaveAsync();
    }
}
