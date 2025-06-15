using Microsoft.AspNetCore.Http;

namespace Shared.DataTransferObjects
{
    public record EquipmentDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool IsRentable { get; set; }
        public decimal? Price { get; set; }
        public int Quantity { get; set; }
        public List<string>? PhotoUrls { get; set; }
        public string? Description { get; set; }
    }

    public record EquipmentForCreationDto
    {
        public string Name { get; set; }
        public bool IsRentable { get; set; }
        public decimal? Price { get; set; }
        public int Quantity { get; set; }
        public List<IFormFile>? PhotoFiles { get; set; } 
        public string? Description { get; set; }
    }

    public record EquipmentForUpdateDto
    {
        public string Name { get; set; }
        public bool IsRentable { get; set; }
        public decimal? Price { get; set; }
        public int Quantity { get; set; }
        public List<IFormFile>? PhotoFiles { get; set; }
        public string? Description { get; set; }
    }
}