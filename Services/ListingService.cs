using ikincelwebsitesi.Data;
using ikincelwebsitesi.Interfaces;
using ikincelwebsitesi.Models;
using ikincelwebsitesi.Models.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace ikincelwebsitesi.Services
{
    public class ListingService : IListingService
    {
        private readonly ApplicationDbContext _context;
        private readonly IFileService _fileService;

        public ListingService(ApplicationDbContext context, IFileService fileService)
        {
            _context = context;
            _fileService = fileService;
        }

        public async Task<PagedList<Listing>> GetListingsAsync(int page, int pageSize, string? search, decimal? minPrice, decimal? maxPrice, int? categoryId)
        {
            var query = _context.Listings.Include(l => l.Category).Include(l => l.User).AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
                query = query.Where(l => l.Title.Contains(search) || l.Description.Contains(search));

            if (minPrice.HasValue)
                query = query.Where(l => l.Price >= minPrice.Value);

            if (maxPrice.HasValue)
                query = query.Where(l => l.Price <= maxPrice.Value);

            if (categoryId.HasValue && categoryId.Value > 0)
                query = query.Where(l => l.CategoryId == categoryId.Value);

            var totalCount = await query.CountAsync();
            var items = await query.OrderByDescending(l => l.CreatedAt)
                                   .Skip((page - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            return new PagedList<Listing>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = page,
                PageSize = pageSize
            };
        }

        public async Task<Listing?> GetListingByIdAsync(int id)
        {
            return await _context.Listings
                .Include(l => l.Category)
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<Listing> CreateListingAsync(Listing listing, IFormFile? imageFile)
        {
            if (imageFile != null)
            {
                listing.ImageUrl = await _fileService.SaveFileAsync(imageFile, "uploads");
            }
            
            _context.Listings.Add(listing);
            await _context.SaveChangesAsync();
            return listing;
        }

        public async Task<bool> UpdateListingAsync(Listing listing, IFormFile? imageFile)
        {
            var existing = await _context.Listings.FindAsync(listing.Id);
            if (existing == null) return false;

            existing.Title = listing.Title;
            existing.Description = listing.Description;
            existing.Price = listing.Price;
            existing.CategoryId = listing.CategoryId;

            if (imageFile != null)
            {
                if (!string.IsNullOrEmpty(existing.ImageUrl))
                {
                    _fileService.DeleteFile(existing.ImageUrl, "uploads");
                }
                existing.ImageUrl = await _fileService.SaveFileAsync(imageFile, "uploads");
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteListingAsync(int id)
        {
            var listing = await _context.Listings.FindAsync(id);
            if (listing == null) return false;

            if (!string.IsNullOrEmpty(listing.ImageUrl))
            {
                _fileService.DeleteFile(listing.ImageUrl, "uploads");
            }

            _context.Listings.Remove(listing);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        public async Task<bool> IsListingOwnedByUserAsync(int listingId, string userId)
        {
            return await _context.Listings.AnyAsync(l => l.Id == listingId && l.UserId == userId);
        }
    }
}
