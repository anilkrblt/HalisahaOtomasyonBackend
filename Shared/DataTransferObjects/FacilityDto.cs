// Shared.DataTransferObjects/FacilityDtos.cs
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Shared.DataTransferObjects;

public record FacilityDto
{

    public int OwnerId { get; set; }  

    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? LogoUrl { get; set; }

    /* Konum */
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }

    public string? City { get; set; }
    public string? Town { get; set; }
    public string? AddressDetails { get; set; }

    /* Özellikler & İmkânlar */
    public bool HasCafeteria { get; set; }
    public bool HasShower { get; set; }
    public bool HasToilet { get; set; }
    public bool HasTransportService { get; set; }


    public bool HasCamera { get; set; }
    public bool HasLockerRoom { get; set; }
    public bool HasFirstAid { get; set; }
    public bool HasSecurityCameras { get; set; }
    public bool HasShoeRental { get; set; }
    public bool HasGlove { get; set; }
    public bool HasParking { get; set; }
    public bool HasRefereeService { get; set; }


    /* İletişim */
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }

    /* Finans & Puan */
    public string? BankAccountInfo { get; set; }
    public double Rating { get; set; }

    /* Alt listeler (isteğe bağlı) */
    public List<FieldDto>? Fields { get; set; }
    public List<EquipmentDto>? Equipments { get; set; }
    public List<string>? PhotoUrls { get; set; }
}

/*────────────── CREATE ──────────────*/
public record FacilityForCreationDto
{
    public int OwnerId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    /* Logo & Fotoğraflar */
    public IFormFile? LogoFile { get; set; }
    [MaxLength(3)]
    public List<IFormFile>? PhotoFiles { get; set; }

    /* Konum */
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public string? City { get; set; }
    public string? Town { get; set; }
    public string? AddressDetails { get; set; }

    /* Özellikler */
    public bool HasLockerRoom { get; set; }
    public bool HasFirstAid { get; set; }
    public bool HasSecurityCameras { get; set; }
    public bool HasCafeteria { get; set; }
    public bool HasShower { get; set; }
    public bool HasToilet { get; set; }
    public bool HasTransportService { get; set; }
    public bool HasParking { get; set; }
    public bool HasRefereeService { get; set; }

    /* İletişim & Finans */
    public string Phone { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? BankAccountInfo { get; set; }

    public List<EquipmentForCreationDto>? Equipments { get; set; }
}

/*────────────── UPDATE (PUT) ──────────────*/
public record FacilityForUpdateDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? BankAccountInfo { get; set; }

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
    public IFormFile? LogoFile { get; set; }
}

/*────────────── PATCH ──────────────*/
public class FacilityPatchDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? BankAccountInfo { get; set; }

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
public record FacilityPhotosUpdateDto(List<IFormFile> PhotoFiles);




