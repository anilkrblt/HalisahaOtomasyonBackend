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
     DateTime? UpdatedAt,
     UserMiniDto User1Info
 );

    public record UserMiniDto(
        int Id,
        string UserName,
        string FullName,
        string? PhotoUrl
    );


    /* İstek gönderme */
    public record FriendRequestForCreationDto(int ToUserId);

    /* İstek yanıtı */
    public record FriendRequestRespondDto(FriendshipStatus Status);

}