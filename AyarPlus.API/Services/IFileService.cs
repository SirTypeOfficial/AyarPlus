namespace AyarPlus.API.Services;

public interface IFileService
{
    Task<string?> SaveFileAsync(IFormFile? file, string subFolder);
    void DeleteFile(string? filePath);
    bool IsValidImage(IFormFile file);
}

