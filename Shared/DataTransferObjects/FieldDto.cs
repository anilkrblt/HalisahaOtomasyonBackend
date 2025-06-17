using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Shared.DataTransferObjects
{
    public class WeeklyOpeningDto
    {
        public int Id { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }

    public class WeeklyOpeningForCreationDto
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }

    public class FieldExceptionDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public bool IsOpen { get; set; }
    }

    public class FieldExceptionForCreationDto
    {
        public DateTime Date { get; set; }
        public bool IsOpen { get; set; }
    }

    public enum FloorType { Dogal, Yapay, Parke, Kum }

    public class FieldDto
    {
        public int OwnerId { get; set; }
        public int FacilityId { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? City { get; set; }
        public string? Town { get; set; }
        public string? AddressDetails { get; set; }
        public bool HasCafeteria { get; set; }
        public bool HasShower { get; set; }
        public bool HasToilet { get; set; }
        public bool HasTransportService { get; set; }
        public bool HasLockerRoom { get; set; }
        public bool HasFirstAid { get; set; }
        public bool HasSecurityCameras { get; set; }
        public bool HasShoeRental { get; set; }
        public bool HasGlove { get; set; }
        public bool HasParking { get; set; }
        public bool HasRefereeService { get; set; }
        public int Id { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Name { get; set; } = null!;
        public bool IsIndoor { get; set; }
        public bool IsAvailable { get; set; } = true;
        public bool HasCamera { get; set; }
        public bool HasTribune { get; set; }
        public bool HasScoreBoard { get; set; }
        public double AvgRating { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public FloorType FloorType { get; set; }
        public int Capacity { get; set; }
        public decimal PricePerHour { get; set; }
        public bool LightingAvailable { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<string>? PhotoUrls { get; set; }
        public List<RoomDto>? Reservations { get; set; }
        public List<WeeklyOpeningDto>? WeeklyOpenings { get; set; }
        public List<FieldExceptionDto>? Exceptions { get; set; }
    }

    public class FieldForCreationDto
    {
        public int FacilityId { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Name { get; set; } = null!;
        public bool IsIndoor { get; set; }
        public bool HasTribune { get; set; }
        public bool HasScoreBoard { get; set; }
        public bool HasCamera { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public FloorType FloorType { get; set; }
        public int Capacity { get; set; }
        public decimal PricePerHour { get; set; }
        public bool LightingAvailable { get; set; }
        public List<WeeklyOpeningForCreationDto>? WeeklyOpenings { get; set; }
        public List<FieldExceptionForCreationDto>? Exceptions { get; set; }
    }

    public class FieldForUpdateDto
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public string Name { get; set; } = null!;
        public bool IsIndoor { get; set; }
        public bool IsAvailable { get; set; }
        public bool HasTribune { get; set; }
        public bool HasScoreBoard { get; set; }
        public bool HasCamera { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public FloorType FloorType { get; set; }
        public int Capacity { get; set; }
        public decimal PricePerHour { get; set; }
        public bool LightingAvailable { get; set; }
        public List<WeeklyOpeningForCreationDto>? WeeklyOpenings { get; set; }
        public List<FieldExceptionForCreationDto>? Exceptions { get; set; }
    }

    public class FieldPatchDto
    {
        public int? Width { get; set; }
        public int? Height { get; set; }
        public string? Name { get; set; }
        public bool? IsIndoor { get; set; }
        public bool? IsAvailable { get; set; }
        public bool? HasCamera { get; set; }
        public bool? HasTribune { get; set; }
        public bool? HasScoreBoard { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public FloorType? FloorType { get; set; }
        public int? Capacity { get; set; }
        public decimal? PricePerHour { get; set; }
        public bool? LightingAvailable { get; set; }
        public DateTime? CreatedAt { get; set; }
        public List<WeeklyOpeningForCreationDto>? WeeklyOpenings { get; set; }
        public List<FieldExceptionForCreationDto>? Exceptions { get; set; }
    }

    public class FieldPhotosUpdateDto
    {
        public List<IFormFile> PhotoFiles { get; set; }
    }
}
