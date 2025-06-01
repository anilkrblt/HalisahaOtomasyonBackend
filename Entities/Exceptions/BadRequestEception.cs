using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Entities.Exceptions
{
    public abstract class BadRequestException : Exception
    {
        protected BadRequestException(string message)
         : base(message)
        {
        }
    }
}