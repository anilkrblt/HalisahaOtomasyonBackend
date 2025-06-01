using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Exceptions
{
    public class AnnouncementNotFoundException :NotFoundException
    {
        public AnnouncementNotFoundException(int announcementId)
        : base($"Announcement with id: {announcementId} doesn't exist in the database.")
        {
        }
    }
}
