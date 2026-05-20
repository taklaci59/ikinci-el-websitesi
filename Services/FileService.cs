using ikincelwebsitesi.Interfaces;

namespace ikincelwebsitesi.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _env;

        public FileService(IWebHostEnvironment env)
        {
            _env = env;
        }

        public async Task<string> SaveFileAsync(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("File is empty");

            if (file.Length > 2 * 1024 * 1024)
                throw new ArgumentException("File size exceeds 2MB limit");

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (ext != ".jpg" && ext != ".png" && ext != ".webp" && ext != ".jpeg")
                throw new ArgumentException("Only .jpg, .jpeg, .png, .webp formats are allowed");

            var fileName = Guid.NewGuid().ToString() + ext;
            var uploadsFolder = Path.Combine(_env.WebRootPath, folder);

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return $"/{folder}/{fileName}";
        }

        public void DeleteFile(string fileName, string folder)
        {
            if (string.IsNullOrEmpty(fileName)) return;

            // filename in DB is like "/uploads/xyz.jpg".
            var nameOnly = Path.GetFileName(fileName);
            var filePath = Path.Combine(_env.WebRootPath, folder, nameOnly);
            
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}
