using ikincelwebsitesi.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ikincelwebsitesi.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IListingService _listingService;
        private readonly IReportService _reportService;

        public AdminController(IListingService listingService, IReportService reportService)
        {
            _listingService = listingService;
            _reportService = reportService;
        }

        public async Task<IActionResult> Dashboard()
        {
            var stats = await _reportService.GetCategoryStatisticsAsync();
            ViewBag.Stats = stats;
            return View();
        }

        public async Task<IActionResult> ManageListings(int page = 1)
        {
            var listings = await _listingService.GetListingsAsync(page, 20, null, null, null, null);
            return View(listings);
        }

        public async Task<IActionResult> ExportExcel()
        {
            var fileBytes = await _reportService.GenerateListingsExcelAsync();
            return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "ilanlar_raporu.xlsx");
        }

        public async Task<IActionResult> ExportPdf()
        {
            var fileBytes = await _reportService.GenerateListingsPdfAsync();
            return File(fileBytes, "application/pdf", "ilanlar_raporu.pdf");
        }
        
        [HttpPost]
        public async Task<IActionResult> DeleteListing(int id)
        {
            await _listingService.DeleteListingAsync(id);
            return RedirectToAction(nameof(ManageListings));
        }
    }
}
