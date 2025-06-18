using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Shared.DataTransferObjects;

public class FacilityDto
{
    public int OwnerId { get; set; }
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? LogoUrl { get; set; }
    public string? Description { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? City { get; set; }
    public string? Town { get; set; }
    public string? AddressDetails { get; set; }
    public bool HasCafeteria { get; set; }
    public bool HasShower { get; set; }
    public bool HasToilet { get; set; }
    public bool HasTransportService { get; set; }
    public bool HasLockableCabinet { get; set; }
    public bool HasCamera { get; set; }
    public bool HasLockerRoom { get; set; }
    public bool HasFirstAid { get; set; }
    public bool HasSecurityCameras { get; set; }
    public bool HasShoeRental { get; set; }
    public bool HasGlove { get; set; }
    public bool HasParking { get; set; }
    public bool HasRefereeService { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? BankAccountInfo { get; set; }
    public double Rating { get; set; }
    public List<FieldDto>? Fields { get; set; }
    public List<EquipmentDto>? Equipments { get; set; }
    public List<string>? PhotoUrls { get; set; }
}

public class FacilityForCreationDto
{
    public int OwnerId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public IFormFile? LogoFile { get; set; }

    [MaxLength(3)]
    public List<IFormFile>? PhotoFiles { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? City { get; set; }
    public string? Town { get; set; }
    public string? AddressDetails { get; set; }
    public bool HasLockableCabinet { get; set; }
    public bool HasLockerRoom { get; set; }
    public bool HasFirstAid { get; set; }
    public bool HasSecurityCameras { get; set; }
    public bool HasCafeteria { get; set; }
    public bool HasShower { get; set; }
    public bool HasToilet { get; set; }
    public bool HasTransportService { get; set; }
    public bool HasParking { get; set; }
    public bool HasRefereeService { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? BankAccountInfo { get; set; }
    public List<EquipmentForCreationDto>? Equipments { get; set; }
}

public class FacilityForUpdateDto
{
    public int OwnerId { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string BankAccountInfo { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? City { get; set; }
    public string? Town { get; set; }
    public string? AddressDetails { get; set; }
    public bool HasLockableCabinet { get; set; }
    public bool HasLockerRoom { get; set; }
    public bool HasFirstAid { get; set; }
    public bool HasSecurityCameras { get; set; }
    public bool HasCafeteria { get; set; }
    public bool HasShower { get; set; }
    public bool HasToilet { get; set; }
    public bool HasTransportService { get; set; }
    public bool HasParking { get; set; }
    public bool HasRefereeService { get; set; }

    [EmailAddress]
    public string? Email { get; set; }

    [MaxLength(32)]
    public string Phone { get; set; } = string.Empty;
    public IFormFile? LogoFile { get; set; }
    public double Rating { get; set; }
}

public class FacilityPatchDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? BankAccountInfo { get; set; }
    public bool? HasLockableCabinet { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? City { get; set; }
    public string? Town { get; set; }
    public string? AddressDetails { get; set; }
    public bool? HasLockerRoom { get; set; }
    public bool? HasFirstAid { get; set; }
    public bool? HasSecurityCameras { get; set; }
    public bool? HasCafeteria { get; set; }
    public bool? HasShower { get; set; }
    public bool? HasToilet { get; set; }
    public bool? HasTransportService { get; set; }
    public bool? HasParking { get; set; }
    public bool? HasRefereeService { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
}
public class FacilityPhotosUpdateDto
{
    public List<IFormFile> PhotoFiles { get; set; }
}