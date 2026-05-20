using ikincelwebsitesi.Interfaces;
using ikincelwebsitesi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ikincelwebsitesi.Controllers
{
    public class ListingsController : Controller
    {
        private readonly IListingService _listingService;

        public ListingsController(IListingService listingService)
        {
            _listingService = listingService;
        }

        public async Task<IActionResult> Index(int page = 1, string? search = null, decimal? minPrice = null, decimal? maxPrice = null, int? categoryId = null)
        {
            ViewBag.Categories = await _listingService.GetCategoriesAsync();
            ViewBag.Search = search;
            ViewBag.MinPrice = minPrice;
            ViewBag.MaxPrice = maxPrice;
            ViewBag.CategoryId = categoryId;

            var listings = await _listingService.GetListingsAsync(page, 12, search, minPrice, maxPrice, categoryId);
            return View(listings);
        }

        public async Task<IActionResult> Details(int id)
        {
            var listing = await _listingService.GetListingByIdAsync(id);
            if (listing == null) return NotFound();
            return View(listing);
        }

        [Authorize]
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _listingService.GetCategoriesAsync();
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Listing listing, IFormFile? imageFile)
        {
            listing.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            
            ModelState.Remove("User");
            ModelState.Remove("Category");
            ModelState.Remove("UserId");

            if (ModelState.IsValid)
            {
                await _listingService.CreateListingAsync(listing, imageFile);
                return RedirectToAction(nameof(Index));
            }
            
            ViewBag.Categories = await _listingService.GetCategoriesAsync();
            return View(listing);
        }

        [Authorize]
        public async Task<IActionResult> Edit(int id)
        {
            var listing = await _listingService.GetListingByIdAsync(id);
            if (listing == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (listing.UserId != userId && !User.IsInRole("Admin")) return Forbid();

            ViewBag.Categories = await _listingService.GetCategoriesAsync();
            return View(listing);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Listing listing, IFormFile? imageFile)
        {
            var existing = await _listingService.GetListingByIdAsync(listing.Id);
            if (existing == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (existing.UserId != userId && !User.IsInRole("Admin")) return Forbid();

            ModelState.Remove("User");
            ModelState.Remove("Category");
            ModelState.Remove("UserId");
            ModelState.Remove("ImageUrl");
            
            if (ModelState.IsValid)
            {
                await _listingService.UpdateListingAsync(listing, imageFile);
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Categories = await _listingService.GetCategoriesAsync();
            return View(listing);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var existing = await _listingService.GetListingByIdAsync(id);
            if (existing == null) return NotFound();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (existing.UserId != userId && !User.IsInRole("Admin")) return Forbid();

            await _listingService.DeleteListingAsync(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
