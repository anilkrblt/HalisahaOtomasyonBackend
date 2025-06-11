// HalisahaOtomasyonBackend.Tests/RoomServiceTests.cs

using AutoMapper;
using Entities.Models;               // Field, Room, RoomParticipant, vs.
using FluentAssertions;              // FluentAssertions
using Microsoft.EntityFrameworkCore; // .Include(...) ve InMemoryDatabase
using Moq;                           // Mock<INotificationService>
using Repository;                    // RepositoryContext, RepositoryManager
using Service;                       // RoomService
using Service.Contracts;             // IRoomService
using Service.Utilities;
using Shared.DataTransferObjects;    // RoomCreateDto, RoomDto, MatchDto, vs.

namespace HalisahaOtomasyonBackend.Tests
{
    public class RoomServiceTests
    {
        private DbContextOptions<RepositoryContext> CreateInMemoryOptions()
        {
            return new DbContextOptionsBuilder<RepositoryContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
        }

        private IMapper GetMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                //
                // 1) DTO → Entity: RoomCreateDto → Room
                //
                cfg.CreateMap<RoomCreateDto, Room>()
                   .ForMember(dest => dest.AccessType, opt => opt.MapFrom(src => src.AccessType))
                   .ForMember(dest => dest.MaxPlayers, opt => opt.MapFrom(src => src.MaxPlayers));

                //
                // 2) RoomParticipant Entity → RoomParticipantDto
                //
                cfg.CreateMap<RoomParticipant, RoomParticipantDto>();

                //
                // 3) Room Entity → RoomDto
                //
                cfg.CreateMap<Room, RoomDto>()
                   // DTO’daki “RoomId” alanına; entity’deki “Id”’yi geçiriyoruz
                   .ForMember(d => d.RoomId, opt => opt.MapFrom(src => src.Id))
                   // DTO’daki “SlotEnd” = entity’deki “SlotEnd”
                   .ForMember(d => d.SlotEnd, opt => opt.MapFrom(src => src.SlotEnd))
                   // DTO’daki “Status” = entity’deki “Status”
                   .ForMember(d => d.Status, opt => opt.MapFrom(src => src.Status))
                   // DTO’daki “Match” = entity’deki “Match” (Match → MatchDto şeklinde çevrilecek)
                   .ForMember(d => d.Match, opt => opt.MapFrom(src => src.Match))
                   // Geriye kalan property’ler FieldId, SlotStart, AccessType, JoinCode, MaxPlayers, PricePerPlayer, CreatedAt birebir aynı
                   ;

                //
                // 4) Match Entity → MatchDto
                //
                cfg.CreateMap<Entities.Models.Match, MatchDto>()
                   // DTO’daki “DateTime” alanına; entity’deki “StartTime” geçiriyoruz
                   // Eğer DTO’nuzun içinde farklı bir isim varsa onu kullanın (örneğin StartTime veya ScheduledTime)
                   .ForMember(d => d.StartTime, opt => opt.MapFrom(src => src.StartTime))
                   // DTO’daki “FieldId” = entity’deki “Room.FieldId”
                   .ForMember(d => d.FieldId, opt => opt.MapFrom(src => src.Room.FieldId))
                   // DTO’daki “RoomId” = entity’deki “RoomId”
                   // HomeTeamId ve AwayTeamId zaten aynı isimdese ekstra bir şey yok
                   ;
            });

            return config.CreateMapper();
        }

        [Fact]
        public async Task CreateRoomAsync_Should_Create_Room_In_Database()
        {
            // 1) In‐Memory context ve RepositoryManager oluştur
            var options = CreateInMemoryOptions();
            using var ctx = new RepositoryContext(options);
            var repoManager = new RepositoryManager(ctx);

            // 2) Kod üreteci / notifier / mapper mock veya gerçek nesne
            var mockNotif = new Mock<INotificationService>();
            var codeGen = new RandomCodeGenerator(); // Varsa kendi implementasyonunuzu kullanın
            var mapper = GetMapper();

            // 3) RoomService’i yarat
            var roomService = new RoomService(repoManager, mockNotif.Object, codeGen, mapper);

            //
            // 4) “Facility” ve “Field” kaydı ekleyin. CreateRoomAsync içinde
            //    ValidateSlotAgainstField kontrolü var, bu yüzden haftalık açılışı da ekleyelim.
            //
            var facility = new Facility
            {
                Name = "TestFacility",
                Email = "test@facility.com",
                Phone = "1234567890",
            };
            ctx.Facilities.Add(facility);
            await ctx.SaveChangesAsync();

            var field = new Field
            {
                FacilityId = facility.Id,
                Width = 100,
                Height = 50,
                Name = "TestField",
                IsIndoor = false,
                IsAvailable = true,
                HasCamera = false,
                FloorType = Entities.Models.FloorType.Yapay,
                Capacity = 20,
                PricePerHour = 100m,
                LightingAvailable = true,
                CreatedAt = DateTime.UtcNow
            };
            ctx.Fields.Add(field);

            // Haftanın bir günü için açılış ekleyelim ki “ValidateSlotAgainstField” testi geçsin
            ctx.WeeklyOpenings.Add(new WeeklyOpening
            {
                Field = field,
                DayOfWeek = DateTime.UtcNow.AddDays(1).DayOfWeek,
                StartTime = new TimeSpan(17, 0, 0),
                EndTime = new TimeSpan(23, 0, 0)
            });
            await ctx.SaveChangesAsync();

            //
            // 5) RoomCreateDto tanımla
            //
            var slotDate = DateTime.UtcNow.AddDays(1).Date.AddHours(18);
            var dto = new RoomCreateDto();

            // 6) CreateRoomAsync metodunu çağır
            var result = await roomService.CreateRoomAsync(dto, creatorTeamId: 1);

            // 7) Sonucu assert et
            result.Should().NotBeNull();
            result.FieldId.Should().Be(field.Id);
            result.MaxPlayers.Should().Be(4);
            result.AccessType.Should().Be(RoomAccessType.Public);

            // 8) Veritabanındaki kaydı kontrol edelim (“ctx.Rooms” DbSet’ine erişiyoruz)
            var createdRoom = ctx.Rooms
                .Include(r => r.Participants)
                .Include(r => r.Match)
                .FirstOrDefault(r => r.Id == result.RoomId);

            createdRoom.Should().NotBeNull();
            createdRoom!.Participants.Count.Should().Be(1);
            createdRoom.SlotStart.Should().Be(slotDate);
        }

        [Fact]
        public async Task JoinRoomAsync_Should_Add_Participant_When_Capacity_Allows()
        {
            // 1) In‐Memory context ve RepositoryManager
            var options = CreateInMemoryOptions();
            using var ctx = new RepositoryContext(options);
            var repoManager = new RepositoryManager(ctx);

            var mockNotif = new Mock<INotificationService>();
            var codeGen = new RandomCodeGenerator();
            var mapper = GetMapper();
            var roomService = new RoomService(repoManager, mockNotif.Object, codeGen, mapper);

            //
            // 2) Önce bir Facility, Field ve yeni bir Room ekleyelim
            //
            var facility = new Facility
            {
                Name = "TestFacility",
                Email = "test2@facility.com",
                Phone = "0987654321"
            };
            ctx.Facilities.Add(facility);
            await ctx.SaveChangesAsync();

            var field = new Field
            {
                FacilityId = facility.Id,
                Width = 80,
                Height = 40,
                Name = "TestField2",
                IsIndoor = false,
                IsAvailable = true,
                HasCamera = false,
                FloorType = Entities.Models.FloorType.Dogal,
                Capacity = 10,
                PricePerHour = 75m,
                LightingAvailable = true,
                CreatedAt = DateTime.UtcNow
            };
            ctx.Fields.Add(field);

            // Haftalık açılış ekleyelim
            ctx.WeeklyOpenings.Add(new WeeklyOpening
            {
                Field = field,
                DayOfWeek = DateTime.UtcNow.AddDays(1).DayOfWeek,
                StartTime = new TimeSpan(17, 0, 0),
                EndTime = new TimeSpan(23, 0, 0)
            });
            await ctx.SaveChangesAsync();

            // “Room” entitesini ekleyelim
            var room = new Room
            {
                FieldId = field.Id,
                SlotStart = DateTime.UtcNow.AddDays(1).Date.AddHours(19),
                AccessType = RoomAccessType.Public,
                MaxPlayers = 2,
                PricePerPlayer = 30m,
                Match = new Entities.Models.Match() // Tam nitelikli kullandık
            };
            ctx.Rooms.Add(room);
            await ctx.SaveChangesAsync();

            //
            // 3) Kurucu (homeTeam) katılımcıyı önce ekleyelim
            //
            ctx.RoomParticipants.Add(new RoomParticipant
            {
                RoomId = room.Id,
                TeamId = 10,
                IsHome = true,
                Status = ParticipantStatus.Accepted
            });
            await ctx.SaveChangesAsync();

            //
            // 4) Şimdi ikinci takım (teamId=20) JoinRoomAsync ile katılsın
            //
            var participantDto = await roomService.JoinRoomAsync(room.Id, teamId: 20);

            participantDto.Should().NotBeNull();
            participantDto.TeamId.Should().Be(20);

            var participants = ctx.RoomParticipants
                .Where(p => p.RoomId == room.Id)
                .ToList();
            participants.Count.Should().Be(2);
        }
    }
}
