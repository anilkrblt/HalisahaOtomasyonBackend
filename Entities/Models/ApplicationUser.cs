using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Entities.Models
{
    public abstract class ApplicationUser : IdentityUser<int>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string? Location { get; set; }
        public bool Gender { get; set; }       // 0=Male 1=Female
        public DateTime? Birthday { get; set; }
        public string? City { get; set; }
        public string? Town { get; set; }

    }
}