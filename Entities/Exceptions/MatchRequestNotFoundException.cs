using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Exceptions
{
    public class MatchRequestNotFoundException : NotFoundException
    {
        public MatchRequestNotFoundException(int id)
      : base($"MatchRequest with id: {id} doesn't exist in the database.")
        {
        }
    }
}
