using Microsoft.AspNetCore.Http;

namespace EasyGamesWeb
{
    public class FileService
    {
        private readonly string _rootFolder;

        public FileService(IWebHostEnvironment env)
        {
            _rootFolder = Path.Combine(env.WebRootPath ?? "wwwroot", "uploads");
            if (!Directory.Exists(_rootFolder))
            {
                Directory.CreateDirectory(_rootFolder);
            }
        }

        public async Task<string> SaveFile(IFormFile file, string[] allowedExtensions)
        {
            if (file == null || file.Length == 0)
                throw new FileNotFoundException("No file provided.");

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(ext))
                throw new InvalidOperationException("Invalid file type.");

            var fileName = $"{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(_rootFolder, fileName);

            using (var fs = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(fs);
            }

            return fileName; 
        }

        public void DeleteFile(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName)) return;
            var fullPath = Path.Combine(_rootFolder, fileName);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }
    }
}
