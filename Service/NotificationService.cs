using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.SignalR;
using Service.Contracts;
using Service.Hubs;
using Shared.DataTransferObjects;

namespace Service
{
    public class NotificationService : INotificationService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILoggerManager _loggerManager;
        private readonly IMapper _mapper;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(
            IRepositoryManager manager,
            ILoggerManager logger,
            IMapper mapper,
            IHubContext<NotificationHub> hubContext)
        {
            _repositoryManager = manager;
            _loggerManager = logger;
            _mapper = mapper;
            _hubContext = hubContext;
        }

        public async Task<IEnumerable<NotificationDto>> GetAllNotificationsAsync()
        {
            var notifs = await _repositoryManager.Notification.GetAllNotificationsAsync(trackChanges: false);
            return _mapper.Map<IEnumerable<NotificationDto>>(notifs);
        }

        public async Task<NotificationDto> CreateNotificationAsync(NotificationForCreationDto dto)
        {
            var entity = _mapper.Map<Notification>(dto);
            entity.CreatedAt = DateTime.UtcNow;

            _repositoryManager.Notification.CreateNotification(entity);
            await _repositoryManager.SaveAsync();

            var notifDto = _mapper.Map<NotificationDto>(entity);

            // 🔔 SignalR ile kullanıcıya bildirimi canlı gönder
            if (entity.UserId.HasValue)
            {
                await _hubContext.Clients
                    .User(entity.UserId.Value.ToString())
                    .SendAsync("ReceiveNotification", notifDto);
            }

            return notifDto;
        }

        public async Task<NotificationDto> GetNotificationAsync(int id)
        {
            var notif = await _repositoryManager.Notification.GetOneNotificationAsync(id, false)
                         ?? throw new Exception("Notification not found");
            return _mapper.Map<NotificationDto>(notif);
        }

        public async Task<IEnumerable<NotificationDto>> GetNotificationsByUserIdAsync(int userId)
        {
            var notifs = await _repositoryManager.Notification.GetNotificationsByCustomerIdAsync(userId, false);
            return _mapper.Map<IEnumerable<NotificationDto>>(notifs);
        }

        public async Task<IEnumerable<NotificationDto>> GetNotificationsByFacilityIdAsync(int facilityId)
        {
            var notifs = await _repositoryManager.Notification.GetNotificationsByFacilityIdAsync(facilityId, false);
            return _mapper.Map<IEnumerable<NotificationDto>>(notifs);
        }

        public async Task<IEnumerable<NotificationDto>> GetUnreadNotificationsAsync(int userId)
        {
            var notifs = await _repositoryManager.Notification.GetUnreadNotificationsAsync(userId);
            return _mapper.Map<IEnumerable<NotificationDto>>(notifs);
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            await _repositoryManager.Notification.MarkAsReadAsync(notificationId);
            await _repositoryManager.SaveAsync();
        }

        public async Task DeleteNotificationAsync(int id)
        {
            var notif = await _repositoryManager.Notification.GetOneNotificationAsync(id, true)
                         ?? throw new Exception("Notification not found");
            _repositoryManager.Notification.DeleteNotification(notif);
            await _repositoryManager.SaveAsync();
        }
    }
}
