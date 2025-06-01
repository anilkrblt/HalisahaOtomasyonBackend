using Entities.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace Shared.DataTransferObjects
{
    public record AnnouncementDto
    {
        public int Id { get; set; }
        public int FacilityId { get; set; }
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? BannerUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime EndTime { get; set; }
    }

    public record AnnouncementForUpdateDto
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public DateTime EndTime { get; set; }
    }

    public record AnnouncementForCreationDto
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public List<IFormFile> PhotoFile { get; set; }

        public DateTime EndTime { get; set; }
    }
}


