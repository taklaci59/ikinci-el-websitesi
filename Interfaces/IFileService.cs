namespace ikincelwebsitesi.Interfaces
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file, string folder);
        void DeleteFile(string fileName, string folder);
    }
}
