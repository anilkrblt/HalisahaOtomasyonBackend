// Service/FacilityService.cs  – tamamı

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
    private readonly IPhotoService _photo;   // <— burada tutuluyor

    public FacilityService(IRepositoryManager repo,
                           ILoggerManager log,
                           IMapper map,
                           IPhotoService photoService)   // <— DI
    {
        _repo = repo;
        _log = log;
        _map = map;
        _photo = photoService;
    }

    /*───────────────────── CRUD ─────────────────────*/

    /* CREATE ----------------------------------------------------*/
    public async Task<FacilityDto> CreateFacilityAsync(FacilityForCreationDto dto)
    {
        var entity = _map.Map<Facility>(dto);
        _repo.Facility.CreateFacility(entity);
        await _repo.SaveAsync();                // Id elde edildi

        /* Logo */
        if (dto.LogoFile is not null)
            entity.LogoUrl = await _photo.UploadLogoAsync(dto.LogoFile, $"facility/{entity.Id}");

        /* Fotoğraflar (max 3) */
        if (dto.PhotoFiles?.Any() == true)
            await _photo.UploadPhotosAsync(dto.PhotoFiles, "facility", entity.Id);

        await _repo.SaveAsync();
        return _map.Map<FacilityDto>(entity);
    }

    /* READ ------------------------------------------------------*/
    public async Task<IEnumerable<FacilityDto>> GetAllFacilitiesAsync(bool track) =>
        _map.Map<IEnumerable<FacilityDto>>(await _repo.Facility.GetAllFacilitiesAsync(track));

    public async Task<FacilityDto> GetFacilityAsync(int id, bool track) =>
        _map.Map<FacilityDto>(
            await _repo.Facility.GetFacilityAsync(id, track)
            ?? throw new FacilityNotFoundException(id));

    public async Task<IEnumerable<FacilityDto>> GetFacilitiesByOwnerIdAsync(int ownerId, bool track)
    {
        var list = await _repo.Facility.GetAllFacilitiesAsync(track);
        return _map.Map<IEnumerable<FacilityDto>>(list.Where(f => f.OwnerId == ownerId));
    }

    /* UPDATE (PUT) ---------------------------------------------*/
    public async Task UpdateFacilityAsync(int id, FacilityForUpdateDto dto, bool track)
    {
        var fac = await _repo.Facility.GetFacilityAsync(id, track)
                  ?? throw new FacilityNotFoundException(id);

        _map.Map(dto, fac);

        if (dto.LogoFile is not null)
            fac.LogoUrl = await _photo.UploadLogoAsync(dto.LogoFile, $"facility/{id}");

        await _repo.SaveAsync();
    }

    /* PATCH -----------------------------------------------------*/
    public async Task PatchFacilityAsync(int id, FacilityPatchDto patch)
    {
        var fac = await _repo.Facility.GetFacilityAsync(id, true)
                  ?? throw new FacilityNotFoundException(id);

        // manuel patch
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
        if (patch.HasToilet is not null) fac.HasToilet = patch.HasToilet.Value;
        if (patch.HasTransportService is not null) fac.HasTransportService = patch.HasTransportService.Value;
        if (patch.ParkingLot is not null) fac.ParkingLot = patch.ParkingLot.Value;
        if (patch.Email is not null) fac.Email = patch.Email;
        if (patch.Phone is not null) fac.Phone = patch.Phone;

        await _repo.SaveAsync();
    }

    /* DELETE ----------------------------------------------------*/
    public async Task DeleteFacility(int id, bool track)
    {
        var fac = await _repo.Facility.GetFacilityAsync(id, track)
                  ?? throw new FacilityNotFoundException(id);

        await _photo.DeletePhotosByEntityAsync("facility", id, false); // logo+foto

        _repo.Facility.DeleteFacility(fac);
        await _repo.SaveAsync();
    }
}
