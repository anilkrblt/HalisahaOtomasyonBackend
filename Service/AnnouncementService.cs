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
        private readonly IPhotoService _photoService;
        private readonly IMapper _mapper;

        public AnnouncementService(IRepositoryManager manager, IPhotoService photoService, IMapper mapper)
        {
            _repositoryManager = manager;
            _photoService = photoService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AnnouncementDto>> GetAllAnnouncementsAsync(int? facilityId, bool trackChanges)
        {
            var announcements = facilityId is null
                ? (await _repositoryManager.Announcement.GetAnnouncementsAsync(trackChanges)).ToList()
                : (await _repositoryManager.Announcement.GetAnnouncementsForFacilityAsync(facilityId.Value, trackChanges)).ToList();

            var announcementDtos = _mapper.Map<List<AnnouncementDto>>(announcements);

            foreach (var announcementDto in announcementDtos)
            {
                var photos = await _photoService.GetPhotosAsync("announcement", announcementDto.Id, false);
                var photo = photos.FirstOrDefault();
                announcementDto.BannerUrl = photo?.Url;
            }

            return announcementDtos;
        }

        public async Task<AnnouncementDto> GetAnnouncementAsync(int announcementId, bool trackChanges)
        {
            var announcement = await CheckAnnouncementExists(announcementId, trackChanges);
            return _mapper.Map<AnnouncementDto>(announcement);
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

        public async Task UpdateAnnouncement(int announcementId, AnnouncementForUpdateDto announcementDto, bool trackChanges)
        {
            var entity = await CheckAnnouncementExists(announcementId, trackChanges);

            _mapper.Map(announcementDto, entity);
            await _repositoryManager.SaveAsync();
        }

        public async Task DeleteAnnouncement(int announcementId, bool trackChanges)
        {
            var announcement = await CheckAnnouncementExists(announcementId, trackChanges);

            _repositoryManager.Announcement.DeleteAnnouncement(announcement);
            await _repositoryManager.SaveAsync();
        }

        private async Task<Announcement> CheckAnnouncementExists(int announcementId, bool trackChanges)
        {
            var announcement = await _repositoryManager.Announcement.GetAnnouncementAsync(announcementId, trackChanges);
            if (announcement is null)
                throw new AnnouncementNotFoundException(announcementId);

            return announcement;
        }
    }
}
