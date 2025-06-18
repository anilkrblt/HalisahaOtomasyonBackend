using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Entities.Models;

namespace Contracts
{
    public interface IRoomRepository
    {
        void CreateRoom(Room room);
        void DeleteRoom(Room room);


        Task<Room?> GetRoomWithParticipantsAsync(int roomId, bool trackChanges);

        Task<IEnumerable<Room>> GetAllRoomsAsync(bool trackChanges);

        Task<IEnumerable<Room>> GetRoomsByFacilityIdAsync(int facilityId, bool trackChanges);
        Task<IEnumerable<Room>> GetRoomsByUserIdAsync(int userId, bool trackChanges);


        // Belirli bir oda getir (trackChanges: EF tracking)
        Task<Room?> GetOneRoomAsync(int roomId, bool trackChanges);

        // Saha bazlı oda listesi
        Task<IEnumerable<Room>> GetRoomsByFieldIdAsync(int fieldId, bool trackChanges);

        // "Public" erişim tipine göre oda listesi
        Task<IEnumerable<Room>> GetPublicRoomsAsync(RoomAccessType accessType);

        // JoinCode’a göre özel oda getir
        Task<Room?> GetRoomByCodeAsync(string joinCode, bool trackChanges);
    }

}