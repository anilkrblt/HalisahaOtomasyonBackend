using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;

namespace Service
{
    public class AnnouncementService : IAnnouncementService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly ILoggerManager _loggerManager;
        private readonly IMapper _mapper;

        public AnnouncementService(IRepositoryManager manager, ILoggerManager logger, IMapper mapper)
        {
            _repositoryManager = manager;
            _loggerManager = logger;
            _mapper = mapper;
        }

        public async Task<AnnouncementDto> CreateAnnouncementAsync(int facilityId, AnnouncementForCreationDto announcementDto)
        {
            var announcement = _mapper.Map<Announcement>(announcementDto);
            announcement.FacilityId = facilityId;
            announcement.CreatedAt = DateTime.UtcNow;

            _repositoryManager.Announcement.CreateAnnouncement(announcement);
            await _repositoryManager.SaveAsync();
            return _mapper.Map<AnnouncementDto>(announcement);
        }

        public async Task DeleteAnnouncement(int announcementId, bool trackchanges)
        {
            var announcement = await _repositoryManager.Announcement.GetAnnouncementAsync(announcementId, trackchanges);
            if (announcement is null)
            {
                throw new AnnouncementNotFoundException(announcementId);
            }
            _repositoryManager.Announcement.DeleteAnnouncement(announcement);
            await _repositoryManager.SaveAsync();
        }

        public async Task<IEnumerable<AnnouncementDto>> GetAllAnnouncementsAsync(bool trackChanges)
        {
            var announcement = await _repositoryManager.Announcement.GetAnnouncementsAsync(trackChanges);
            return _mapper.Map<IEnumerable<AnnouncementDto>>(announcement);
        }

        public async Task<AnnouncementDto> GetAnnouncementAsync(int announcementId, bool trackChanges)
        {
            var entity = await _repositoryManager.Announcement.GetAnnouncementAsync(announcementId, trackChanges);
            if (entity is null)
            {
                throw new AnnouncementNotFoundException(announcementId);
            }
            return _mapper.Map<AnnouncementDto>(entity);
        }

        public async Task UpdateAnnouncement(int announcementId, AnnouncementForUpdateDto announcement, bool trackChanges)
        {
            var entity = await _repositoryManager.Announcement.GetAnnouncementAsync(announcementId, true);
            if (entity is null)
            {
                throw new AnnouncementNotFoundException(announcementId);
            }

            _mapper.Map(announcement, entity);
            await _repositoryManager.SaveAsync();
        }
    }
}
