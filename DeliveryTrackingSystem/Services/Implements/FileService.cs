using DeliveryTrackingSystem.Services.Interfaces;

namespace DeliveryTrackingSystem.Services.Implements
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly string _baseUploadPath;

        public FileService(IWebHostEnvironment environment, IHttpContextAccessor httpContextAccessor)
        {
            _environment = environment;
            _httpContextAccessor = httpContextAccessor;
            _baseUploadPath = Path.Combine(_environment.WebRootPath, "Images");

            if (!Directory.Exists(_baseUploadPath))
            {
                Directory.CreateDirectory(_baseUploadPath);
            }
        }

        public string SaveFile(IFormFile file, string category)
        {
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!allowedExtensions.Contains(extension))
            {
                throw new InvalidOperationException("Invalid file type. Only JPG, JPEG, and PNG are allowed");
            }

            const long maxFileSize = 3 * 1024 * 1024; //2MB
            if (file.Length > maxFileSize)
            {
                throw new InvalidOperationException("File size exceeds the maximum limit of 3 MB");
            }

            var categoryPath = Path.Combine(_baseUploadPath, category);
            if (!Directory.Exists(categoryPath))
            {
                Directory.CreateDirectory(categoryPath);
            }

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(categoryPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }

            // احصل على عنوان الموقع الحالي
            var request = (_httpContextAccessor.HttpContext?.Request) ??
                throw new InvalidOperationException("HttpContextAccessor Not Valid!");
            //var baseUrl = $"{request?.Scheme}://{request?.Host}";

            // إنشاء الرابط الكامل للصورة
            var baseUrl = $"{request?.Scheme}://{request?.Host}";

            return $"{baseUrl}/Images/{category}/{fileName}";
        }

        public async Task DeleteFileAsync(string fileUrl)
        {
            if (string.IsNullOrEmpty(fileUrl))
            {
                throw new ArgumentException("File path cannot be null or empty");
            }

            // استخراج الجزء النسبي من الرابط
            var relativePath = fileUrl.Replace($"{_httpContextAccessor.HttpContext?.Request.Scheme}://{_httpContextAccessor.HttpContext?.Request.Host}/", "");


            // تحويل المسار إلى مسار فعلي داخل `wwwroot`
            var fullPath = Path.Combine(_environment.WebRootPath, relativePath);

            if (File.Exists(fullPath))
            {
                try
                {
                    await Task.Run(() => File.Delete(fullPath));
                }
                catch (Exception ex)
                {
                    throw new IOException($"Error occurred while deleting the file: {fullPath}");
                }
            }
            //else
            //{
            //	throw new FileNotFoundException($"لم يتم العثور على الملف: {fullPath}");
            //}
        }
    }
}