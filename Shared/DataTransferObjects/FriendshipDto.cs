using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.Models;

namespace Shared.DataTransferObjects
{
    /* Arkadaş satırını döndürürken */
    public class FriendshipDto
    {
        public int UserId1 { get; set; }
        public int UserId2 { get; set; }
        public FriendshipStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public UserMiniDto? User1Info { get; set; } // optional olabilir
    }
    public class UserMiniDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? PhotoUrl { get; set; }
    }


    /* İstek gönderme */
    public record FriendRequestForCreationDto(int ToUserId);

    /* İstek yanıtı */
    public record FriendRequestRespondDto(FriendshipStatus Status);

}