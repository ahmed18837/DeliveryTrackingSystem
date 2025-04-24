namespace DeliveryTrackingSystem.Services.Interfaces
{
    public interface IFileService
    {
        string SaveFile(IFormFile file, string category);
        Task DeleteFileAsync(string relativePath);
    }
}
