using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Entities.Models
{

    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string EntityType { get; set; }  
        public int EntityId { get; set; }       
        public string? Description { get; set; }

    }

}