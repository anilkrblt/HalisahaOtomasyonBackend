using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Entities.Exceptions
{

    public class TeamNotFoundException : NotFoundException
    {
        public TeamNotFoundException(int id) : base($"Takım ({id}) bulunamadı.") { }
    }

    public class TeamMemberNotFoundException : NotFoundException
    {
        public TeamMemberNotFoundException(int teamId, int userId)
            : base($"Takım üyesi [Team:{teamId} User:{userId}] bulunamadı.") { }
    }

    public class JoinRequestNotFoundException : NotFoundException
    {
        public JoinRequestNotFoundException(int id) : base($"Join request ({id}) bulunamadı.") { }
    }

}