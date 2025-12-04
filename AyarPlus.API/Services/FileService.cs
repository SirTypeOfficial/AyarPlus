namespace AyarPlus.API.Services;

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _environment;
    private readonly string[] _allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
    private const long MaxFileSize = 5 * 1024 * 1024; // 5MB

    public FileService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public async Task<string?> SaveFileAsync(IFormFile? file, string subFolder)
    {
        if (file == null || file.Length == 0)
            return null;

        if (!IsValidImage(file))
            throw new InvalidOperationException("Invalid file type or size");

        var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", subFolder);
        
        if (!Directory.Exists(uploadsFolder))
            Directory.CreateDirectory(uploadsFolder);

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var fileName = $"{Guid.NewGuid()}{extension}";
        var filePath = Path.Combine(uploadsFolder, fileName);

        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        return $"/uploads/{subFolder}/{fileName}";
    }

    public void DeleteFile(string? filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            return;

        var fullPath = Path.Combine(_environment.WebRootPath, filePath.TrimStart('/'));
        
        if (File.Exists(fullPath))
            File.Delete(fullPath);
    }

    public bool IsValidImage(IFormFile file)
    {
        if (file.Length > MaxFileSize)
            return false;

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        return _allowedExtensions.Contains(extension);
    }
}

