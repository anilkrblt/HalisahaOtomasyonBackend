using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.Models;

namespace Shared.DataTransferObjects
{
    /* Arkadaş satırını döndürürken */
    public record FriendshipDto(
        int UserId1,
        int UserId2,
        FriendshipStatus Status,
        DateTime CreatedAt,
        DateTime? UpdatedAt);

    /* İstek gönderme */
    public record FriendRequestForCreationDto(int ToUserId);

    /* İstek yanıtı */
    public record FriendRequestRespondDto(FriendshipStatus Status);

}