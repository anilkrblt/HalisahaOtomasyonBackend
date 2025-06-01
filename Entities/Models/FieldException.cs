using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class FieldException
    {
        public int Id { get; set; }
        public int FieldId { get; set; }
        public Field Field { get; set; }

        public DateTime Date { get; set; }
        // o tarih için açık mı kapalı mı
        public bool IsOpen { get; set; }
    }
}