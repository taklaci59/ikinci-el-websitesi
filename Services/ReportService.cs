using ClosedXML.Excel;
using ikincelwebsitesi.Data;
using ikincelwebsitesi.Interfaces;
using Microsoft.EntityFrameworkCore;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;

namespace ikincelwebsitesi.Services
{
    public class ReportService : IReportService
    {
        private readonly ApplicationDbContext _context;

        public ReportService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Dictionary<string, int>> GetCategoryStatisticsAsync()
        {
            return await _context.Categories
                .Select(c => new { c.Name, Count = c.Listings.Count })
                .ToDictionaryAsync(k => k.Name, v => v.Count);
        }

        public async Task<byte[]> GenerateListingsExcelAsync()
        {
            var listings = await _context.Listings.Include(l => l.Category).Include(l => l.User).ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("İlanlar");

            worksheet.Cell(1, 1).Value = "ID";
            worksheet.Cell(1, 2).Value = "Başlık";
            worksheet.Cell(1, 3).Value = "Kategori";
            worksheet.Cell(1, 4).Value = "Fiyat";
            worksheet.Cell(1, 5).Value = "Kullanıcı";
            worksheet.Cell(1, 6).Value = "Tarih";

            var row = 2;
            foreach (var item in listings)
            {
                worksheet.Cell(row, 1).Value = item.Id;
                worksheet.Cell(row, 2).Value = item.Title;
                worksheet.Cell(row, 3).Value = item.Category?.Name;
                worksheet.Cell(row, 4).Value = item.Price;
                worksheet.Cell(row, 5).Value = item.User?.FullName ?? item.User?.Email;
                worksheet.Cell(row, 6).Value = item.CreatedAt.ToString("dd.MM.yyyy");
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        public async Task<byte[]> GenerateListingsPdfAsync()
        {
            var listings = await _context.Listings.Include(l => l.Category).Include(l => l.User).ToListAsync();

            using var stream = new MemoryStream();
            using var writer = new PdfWriter(stream);
            using var pdf = new PdfDocument(writer);
            using var document = new Document(pdf);

            document.Add(new Paragraph("MarketPlacePro İlan Raporu")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(20));

            var table = new Table(5, true);
            table.AddHeaderCell("ID");
            table.AddHeaderCell("Başlık");
            table.AddHeaderCell("Kategori");
            table.AddHeaderCell("Fiyat");
            table.AddHeaderCell("Tarih");

            foreach (var item in listings)
            {
                table.AddCell(item.Id.ToString());
                table.AddCell(item.Title ?? "");
                table.AddCell(item.Category?.Name ?? "");
                table.AddCell(item.Price.ToString("C"));
                table.AddCell(item.CreatedAt.ToString("dd.MM.yyyy"));
            }

            document.Add(table);
            document.Close();

            return stream.ToArray();
        }
    }
}
