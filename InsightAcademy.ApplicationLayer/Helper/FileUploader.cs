using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace InsightAcademy.UI.Helper
{
    public class FileUploader
    {
        private readonly IWebHostEnvironment _environment;

        public FileUploader(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public async Task<(bool status, string message, string url)> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return (false, "Please select a file to upload.", "");
            }

            try
            {
                var uploadsDirectory = Path.Combine(_environment.WebRootPath, "images/profile");

                if (!Directory.Exists(uploadsDirectory))
                {
                    Directory.CreateDirectory(uploadsDirectory);
                }

                var uniqueFileName = $"{Guid.NewGuid().ToString()}_{DateTime.Now:yyyyMMddHHmmssfff}_{file.FileName}";
                var filePath = Path.Combine(uploadsDirectory, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                return (true, "File uploaded successfully!", $"images/profile/{uniqueFileName}");
            }
            catch (Exception ex)
            {
                return (false, $"Error uploading file: {ex.Message}", "");
            }
        }

        public async Task<(bool status, string message, string url)> UploadVideo(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return (false, "Please select a file to upload.", "");
            }

            try
            {
                var uploadsDirectory = Path.Combine(_environment.WebRootPath, "videos");

                if (!Directory.Exists(uploadsDirectory))
                {
                    Directory.CreateDirectory(uploadsDirectory);
                }

                var uniqueFileName = $"{Guid.NewGuid().ToString()}_{DateTime.Now:yyyyMMddHHmmssfff}_{file.FileName}";
                var filePath = Path.Combine(uploadsDirectory, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                return (true, "File uploaded successfully!", $"videos/{uniqueFileName}");
            }
            catch (Exception ex)
            {
                return (false, $"Error uploading file: {ex.Message}", "");
            }
        }
    }
}