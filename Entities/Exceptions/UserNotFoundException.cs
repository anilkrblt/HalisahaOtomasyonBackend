using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Entities.Exceptions
{
    public class UserNotFoundException : NotFoundException
    {
        public UserNotFoundException(int userId)
 : base($"User with id: {userId} doesn't exist in the database.")
        {
        }


    }
}