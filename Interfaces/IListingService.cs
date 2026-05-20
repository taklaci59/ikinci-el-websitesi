using ikincelwebsitesi.Models;
using ikincelwebsitesi.Models.ViewModels;

namespace ikincelwebsitesi.Interfaces
{
    public interface IListingService
    {
        Task<PagedList<Listing>> GetListingsAsync(int page, int pageSize, string? search, decimal? minPrice, decimal? maxPrice, int? categoryId);
        Task<Listing?> GetListingByIdAsync(int id);
        Task<Listing> CreateListingAsync(Listing listing, IFormFile? imageFile);
        Task<bool> UpdateListingAsync(Listing listing, IFormFile? imageFile);
        Task<bool> DeleteListingAsync(int id);
        Task<List<Category>> GetCategoriesAsync();
        Task<bool> IsListingOwnedByUserAsync(int listingId, string userId);
    }
}
