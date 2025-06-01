using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Entities.Exceptions
{
    public class FieldNotFoundException : NotFoundException
    {
        public FieldNotFoundException(int fieldId)
         : base($"Field with id: {fieldId} doesn't exist in the database.")
        {
        }
    }
}