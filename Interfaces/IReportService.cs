namespace ikincelwebsitesi.Interfaces
{
    public interface IReportService
    {
        Task<Dictionary<string, int>> GetCategoryStatisticsAsync();
        Task<byte[]> GenerateListingsExcelAsync();
        Task<byte[]> GenerateListingsPdfAsync();
    }
}
