// Shared/DataTransferObjects/FieldDtos.cs
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;

namespace Shared.DataTransferObjects
{
    // --- Yeni: Haftalık program DTO’ları ---
    public record WeeklyOpeningDto
    {
        public int Id { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }

    public record WeeklyOpeningForCreationDto
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }

    // --- Yeni: Tarihe özel istisna DTO’ları ---
    public record FieldExceptionDto
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public bool IsOpen { get; set; }
    }

    public record FieldExceptionForCreationDto
    {
        public DateTime Date { get; set; }
        public bool IsOpen { get; set; }
    }

    // Var olan Field DTO’ları aşağı gibi güncellendi:
    public enum FloorType { Dogal, Sentetik, Parke }

    public record FieldDto
    {
        public int Id { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Name { get; set; } = null!;
        public bool IsIndoor { get; set; }
        public bool IsAvailable { get; set; } = true;
        public bool HasCamera { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public FloorType FloorType { get; set; }
        public int Capacity { get; set; }

        public decimal PricePerHour { get; set; }
        public bool LightingAvailable { get; set; }
        public DateTime CreatedAt { get; set; }

        // Fotoğraflar + Rezervasyonlar aynen duruyor
        public List<string>? PhotoUrls { get; set; }
        public List<ReservationDto>? Reservations { get; set; }

        // Günlük rutin ve istisnalar
        public List<WeeklyOpeningDto>? WeeklyOpenings { get; set; }
        public List<FieldExceptionDto>? Exceptions { get; set; }
    }

    public record FieldForCreationDto
    {
        public int FacilityId { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string Name { get; set; } = null!;
        public bool IsIndoor { get; set; }
        public bool HasCamera { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public FloorType FloorType { get; set; }
        public int Capacity { get; set; }

        public decimal PricePerHour { get; set; }
        public bool LightingAvailable { get; set; }

        // Yeni alanlar:
        public List<WeeklyOpeningForCreationDto>? WeeklyOpenings { get; set; }
        public List<FieldExceptionForCreationDto>? Exceptions { get; set; }

    }



    public record FieldForUpdateDto
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public string Name { get; set; } = null!;
        public bool IsIndoor { get; set; }
        public bool IsAvailable { get; set; }
        public bool HasCamera { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public FloorType FloorType { get; set; }
        public int Capacity { get; set; }

        public decimal PricePerHour { get; set; }
        public bool LightingAvailable { get; set; }

        // Yeni alanlar:
        public List<WeeklyOpeningForCreationDto>? WeeklyOpenings { get; set; }
        public List<FieldExceptionForCreationDto>? Exceptions { get; set; }
    }

    public record FieldPatchDto
    {
        public int? Width { get; set; }
        public int? Height { get; set; }
        public string? Name { get; set; }
        public bool? IsIndoor { get; set; }
        public bool? IsAvailable { get; set; }
        public bool? HasCamera { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public FloorType? FloorType { get; set; }
        public int? Capacity { get; set; }

        public decimal? PricePerHour { get; set; }
        public bool? LightingAvailable { get; set; }

        // Opsiyonel: hafta programını veya istisnaları patch’leyebilmek için
        public List<WeeklyOpeningForCreationDto>? WeeklyOpenings { get; set; }
        public List<FieldExceptionForCreationDto>? Exceptions { get; set; }
    }

    public record FieldPhotosUpdateDto
    {
        public List<IFormFile> PhotoFiles { get; set; }
    }
}
