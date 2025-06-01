// Service/PhotoService.cs  (tümü bir dosya)
using AutoMapper;
using Contracts;
using Entities.Models;
using Microsoft.AspNetCore.Http;
using Service.Contracts;
using Shared.DataTransferObjects;
using System.IO;

namespace Service;

public class PhotoService : IPhotoService
{
    private readonly IRepositoryManager _repo;
    private readonly IMapper _map;

    private static readonly string[] _allowed = [".jpg", ".jpeg", ".png", ".gif", ".webp"];
    private const int _maxSize = 5 * 1024 * 1024;          // 5 MB

    public PhotoService(IRepositoryManager repo, IMapper map)
    {
        _repo = repo;
        _map = map;
    }

    /*────────────────── PUBLIC API ──────────────────*/

    /* 1) Logo – tek dosya, URL geri döner */
    public async Task<string> UploadLogoAsync(IFormFile file, string entityFolder)
    {
        ValidateFile(file);

        var ext = Path.GetExtension(file.FileName).ToLower();
        var folderAbs = BuildFolderPath(entityFolder);
        Directory.CreateDirectory(folderAbs);

        var logoName = $"logo{ext}";
        var absPath = Path.Combine(folderAbs, logoName);
        await using var fs = new FileStream(absPath, FileMode.Create);
        await file.CopyToAsync(fs);

        return $"/images/{entityFolder.Replace('\\', '/')}/{logoName}";
    }

    /* 2) Çoklu fotoğraf */
    public async Task UploadPhotosAsync(IEnumerable<IFormFile> files,
                                        string entityType,
                                        int entityId,
                                        int? parentId = null)
    {
        var folderRel = entityType.ToLower() == "field" && parentId is not null
            ? Path.Combine("facility", parentId.Value.ToString(), "field", entityId.ToString())
            : Path.Combine(entityType.ToLower(), entityId.ToString());

        var folderAbs = BuildFolderPath(folderRel);
        Directory.CreateDirectory(folderAbs);

        foreach (var file in files.Take(5))
        {
            ValidateFile(file);

            var ext = Path.GetExtension(file.FileName).ToLower();
            var fname = $"{Path.GetFileNameWithoutExtension(file.FileName)
                           .Replace(' ', '_')}_{DateTime.UtcNow:yyyyMMddHHmmssfff}{ext}";
            var abs = Path.Combine(folderAbs, fname);

            await using var stream = new FileStream(abs, FileMode.Create);
            await file.CopyToAsync(stream);

            var rel = BuildRelativePath(folderRel, fname);

            _repo.Photo.CreatePhoto(new Photo
            {
                EntityType = entityType.ToLower(),
                EntityId = entityId,
                Url = rel
            });
        }

        await _repo.SaveAsync();
    }

    /* 3) Listeleme */
    public async Task<IEnumerable<PhotoDto>> GetPhotosAsync(string entityType, int entityId, bool track) =>
        _map.Map<IEnumerable<PhotoDto>>(
            await _repo.Photo.GetAllByEntityAsync(entityType.ToLower(), entityId, track));

    /* 4) Silme (fotoğraflar + logo dâhil) */
    public async Task DeletePhotosByEntityAsync(string entityType, int entityId, bool track)
    {
        var rows = await _repo.Photo.GetAllByEntityAsync(entityType, entityId, track);

        // klasör
        var relFolder = Path.Combine(entityType.ToLower(), entityId.ToString());
        var absFolder = BuildFolderPath(relFolder);
        if (Directory.Exists(absFolder))
            Directory.Delete(absFolder, true);

        foreach (var p in rows)
            _repo.Photo.DeletePhoto(p);

        await _repo.SaveAsync();
    }

    /*────────────────── Yardımcılar ──────────────────*/

    private static void ValidateFile(IFormFile f)
    {
        var ext = Path.GetExtension(f.FileName).ToLower();
        if (!_allowed.Contains(ext))
            throw new InvalidOperationException($"Geçersiz format: {ext}");
        if (f.Length > _maxSize)
            throw new InvalidOperationException("Dosya 5 MB’tan büyük.");
    }

    private static string BuildFolderPath(string relative)
        => Path.Combine(Directory.GetCurrentDirectory(), "..", "HalisahaOtomasyon",
                        "wwwroot", "images", relative);

    private static string BuildRelativePath(string folderRel, string fileName)
        => $"/images/{folderRel.Replace('\\', '/')}/{fileName}";
}
