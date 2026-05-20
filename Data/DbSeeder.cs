using ikincelwebsitesi.Models;
using Microsoft.AspNetCore.Identity;

namespace ikincelwebsitesi.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            // Seed Roles
            string[] roleNames = { "Admin", "User" };
            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            // Seed Admin User
            var adminEmail = "admin@marketplace.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FullName = "System Admin",
                    EmailConfirmed = true
                };

                await userManager.CreateAsync(adminUser, "Admin123!");
            }

            if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }

            // Seed Categories and Listings
            if (!context.Categories.Any())
            {
                var catCar = new Category { Name = "Araba" };
                var catPhone = new Category { Name = "Telefon" };
                var catRealEstate = new Category { Name = "Emlak" };

                context.Categories.AddRange(catCar, catPhone, catRealEstate);
                await context.SaveChangesAsync();

                if (!context.Listings.Any())
                {
                    context.Listings.AddRange(
                        new Listing { Title = "2015 Model Temiz Sedan", Description = "Kazası boyası yoktur.", Price = 450000m, CategoryId = catCar.Id, UserId = adminUser.Id, ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuDeb_QA7dOmSg8n0N-q12Z4bDZwk4GPy-LjV6kE5u7Ekh2tC_Cx88eSRV9J750m4yNRK8dF1BUmUC8A1mMx4rtypjL3U6wrVqcDTCwwzKbMi7CRySlCId6dYHzJ--QfWOetcv-YIrcYQkbSUM7EOojdMqkFAP1jIj054yyb9OoJ_DA7wcZMi44fGRIAklUrKzRzCG6yuYRcZlxphei_cWgJ1_rAWbEU1njjKdxgBPtKq72PFNW8uaBOFu05xxfyHP3RtQbCvPez5LH3" },
                        new Listing { Title = "Acil Satılık SUV", Description = "Bütün bakımları yapılmış temiz SUV.", Price = 850000m, CategoryId = catCar.Id, UserId = adminUser.Id, ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuDeb_QA7dOmSg8n0N-q12Z4bDZwk4GPy-LjV6kE5u7Ekh2tC_Cx88eSRV9J750m4yNRK8dF1BUmUC8A1mMx4rtypjL3U6wrVqcDTCwwzKbMi7CRySlCId6dYHzJ--QfWOetcv-YIrcYQkbSUM7EOojdMqkFAP1jIj054yyb9OoJ_DA7wcZMi44fGRIAklUrKzRzCG6yuYRcZlxphei_cWgJ1_rAWbEU1njjKdxgBPtKq72PFNW8uaBOFu05xxfyHP3RtQbCvPez5LH3" },
                        new Listing { Title = "Son Model Akıllı Telefon", Description = "Kutusunda sıfır.", Price = 35000m, CategoryId = catPhone.Id, UserId = adminUser.Id, ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuCwMiSdV6Ze3nFCnovXfw-iO03Ftzd3koDI4do5QdfckHPE9ETfA74kKaizKQWptGfzBUshmjjTo2Pfe6Dp9lXP4IKYE2Fk7cUGaKcDjr07URVfj6NQr_bi4tEq-yq3j6g31vMPfFEPjwzFR2yV3P-pji3QuUeEes72o5sl5mCQmzg4dAAI6mHNY4Zvg0OJfk2fIVaJ_PY90-qMmKIHmcWA-lFnsKHa3Rt41Xra7GCU0JA8TjxSOAL2Kf5ezrvf8Z_BkWb58khIdRps" },
                        new Listing { Title = "İkinci El Telefon", Description = "Ekranında ufak bir çizik var.", Price = 12000m, CategoryId = catPhone.Id, UserId = adminUser.Id, ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuCwMiSdV6Ze3nFCnovXfw-iO03Ftzd3koDI4do5QdfckHPE9ETfA74kKaizKQWptGfzBUshmjjTo2Pfe6Dp9lXP4IKYE2Fk7cUGaKcDjr07URVfj6NQr_bi4tEq-yq3j6g31vMPfFEPjwzFR2yV3P-pji3QuUeEes72o5sl5mCQmzg4dAAI6mHNY4Zvg0OJfk2fIVaJ_PY90-qMmKIHmcWA-lFnsKHa3Rt41Xra7GCU0JA8TjxSOAL2Kf5ezrvf8Z_BkWb58khIdRps" },
                        new Listing { Title = "Merkezde 3+1 Daire", Description = "Geniş ve ferah daire.", Price = 2500000m, CategoryId = catRealEstate.Id, UserId = adminUser.Id, ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuBzQloGR3ZaCgAV94-z6duQTM4w_vL8_Zy46TBZJ5SDAWPmz5CnLrdYJiT1yJHmPgnYa2nw-Sn0jL3hUEQlXRSYQSmDgTvnbGPSpS_3uY0EpRGAJwNeJOpTN0gLlPQ5B6hqXWV6zHwKZinOw9Hw150zj-Q1k5vpMyyP0nErYgFDXY2xvY9u9IQdn32ejapKe4alHw3hGGS7RR4_XZRZwPn8Vm2yW9Q026-5ZlJAIKXNxRFh02V7PKCYnd6Th-p08xhca2LH3cfNpyjs" },
                        new Listing { Title = "Sahile Yakın Yazlık", Description = "Deniz manzaralı müstakil ev.", Price = 5500000m, CategoryId = catRealEstate.Id, UserId = adminUser.Id, ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuBzQloGR3ZaCgAV94-z6duQTM4w_vL8_Zy46TBZJ5SDAWPmz5CnLrdYJiT1yJHmPgnYa2nw-Sn0jL3hUEQlXRSYQSmDgTvnbGPSpS_3uY0EpRGAJwNeJOpTN0gLlPQ5B6hqXWV6zHwKZinOw9Hw150zj-Q1k5vpMyyP0nErYgFDXY2xvY9u9IQdn32ejapKe4alHw3hGGS7RR4_XZRZwPn8Vm2yW9Q026-5ZlJAIKXNxRFh02V7PKCYnd6Th-p08xhca2LH3cfNpyjs" },
                        new Listing { Title = "Hasarlı Araç", Description = "Parça niyetine satılık.", Price = 50000m, CategoryId = catCar.Id, UserId = adminUser.Id, ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuDeb_QA7dOmSg8n0N-q12Z4bDZwk4GPy-LjV6kE5u7Ekh2tC_Cx88eSRV9J750m4yNRK8dF1BUmUC8A1mMx4rtypjL3U6wrVqcDTCwwzKbMi7CRySlCId6dYHzJ--QfWOetcv-YIrcYQkbSUM7EOojdMqkFAP1jIj054yyb9OoJ_DA7wcZMi44fGRIAklUrKzRzCG6yuYRcZlxphei_cWgJ1_rAWbEU1njjKdxgBPtKq72PFNW8uaBOFu05xxfyHP3RtQbCvPez5LH3" },
                        new Listing { Title = "Oyun Bilgisayarı Takaslık Telefon", Description = "Üst düzey telefon.", Price = 45000m, CategoryId = catPhone.Id, UserId = adminUser.Id, ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuCwMiSdV6Ze3nFCnovXfw-iO03Ftzd3koDI4do5QdfckHPE9ETfA74kKaizKQWptGfzBUshmjjTo2Pfe6Dp9lXP4IKYE2Fk7cUGaKcDjr07URVfj6NQr_bi4tEq-yq3j6g31vMPfFEPjwzFR2yV3P-pji3QuUeEes72o5sl5mCQmzg4dAAI6mHNY4Zvg0OJfk2fIVaJ_PY90-qMmKIHmcWA-lFnsKHa3Rt41Xra7GCU0JA8TjxSOAL2Kf5ezrvf8Z_BkWb58khIdRps" },
                        new Listing { Title = "Stüdyo Daire", Description = "Öğrenciye uygun stüdyo.", Price = 1200000m, CategoryId = catRealEstate.Id, UserId = adminUser.Id, ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuBzQloGR3ZaCgAV94-z6duQTM4w_vL8_Zy46TBZJ5SDAWPmz5CnLrdYJiT1yJHmPgnYa2nw-Sn0jL3hUEQlXRSYQSmDgTvnbGPSpS_3uY0EpRGAJwNeJOpTN0gLlPQ5B6hqXWV6zHwKZinOw9Hw150zj-Q1k5vpMyyP0nErYgFDXY2xvY9u9IQdn32ejapKe4alHw3hGGS7RR4_XZRZwPn8Vm2yW9Q026-5ZlJAIKXNxRFh02V7PKCYnd6Th-p08xhca2LH3cfNpyjs" },
                        new Listing { Title = "Klasik Araba", Description = "Koleksiyonerler için özel.", Price = 1500000m, CategoryId = catCar.Id, UserId = adminUser.Id, ImageUrl = "https://lh3.googleusercontent.com/aida-public/AB6AXuDeb_QA7dOmSg8n0N-q12Z4bDZwk4GPy-LjV6kE5u7Ekh2tC_Cx88eSRV9J750m4yNRK8dF1BUmUC8A1mMx4rtypjL3U6wrVqcDTCwwzKbMi7CRySlCId6dYHzJ--QfWOetcv-YIrcYQkbSUM7EOojdMqkFAP1jIj054yyb9OoJ_DA7wcZMi44fGRIAklUrKzRzCG6yuYRcZlxphei_cWgJ1_rAWbEU1njjKdxgBPtKq72PFNW8uaBOFu05xxfyHP3RtQbCvPez5LH3" }
                    );
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}
