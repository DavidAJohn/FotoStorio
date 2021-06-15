using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using FotoStorio.Shared.Models;
using Microsoft.EntityFrameworkCore;

namespace FotoStorio.Server.Data
{
    public class SeedData
    {
        public static async Task SeedBrandsDataAsync(ApplicationDbContext context)
        {
            if (await context.Brands.AnyAsync()) return;

            var brandsData = await File.ReadAllTextAsync("./Data/SeedData/brands.json");
            var brands = JsonSerializer.Deserialize<List<Brand>>(brandsData);

            using (var transaction = context.Database.BeginTransaction())
            {
                foreach (var brand in brands)
                {
                    context.Brands.Add(brand);
                }

                context.Database.ExecuteSqlInterpolated($"SET IDENTITY_INSERT Brands ON;");
                context.SaveChanges();
                context.Database.ExecuteSqlInterpolated($"SET IDENTITY_INSERT Brands OFF");
                transaction.Commit();
            }
        }

        public static async Task SeedCategoriesDataAsync(ApplicationDbContext context)
        {
            if (await context.Categories.AnyAsync()) return;
            
            var categoriesData = await File.ReadAllTextAsync("./Data/SeedData/categories.json");
            var categories = JsonSerializer.Deserialize<List<Category>>(categoriesData);

            using (var transaction = context.Database.BeginTransaction())
            {
                foreach (var category in categories)
                {
                    context.Categories.Add(category);
                }

                context.Database.ExecuteSqlInterpolated($"SET IDENTITY_INSERT Categories ON;");
                context.SaveChanges();
                context.Database.ExecuteSqlInterpolated($"SET IDENTITY_INSERT Categories OFF");
                transaction.Commit();
            }
        }

        public static async Task SeedProductsDataAsync(ApplicationDbContext context)
        {
            if (await context.Products.AnyAsync()) return;

            var productsData = await File.ReadAllTextAsync("./Data/SeedData/products.json");
            var products = JsonSerializer.Deserialize<List<Product>>(productsData);

            foreach (var product in products)
            {
                context.Products.Add(product);
            }

            await context.SaveChangesAsync();
        }
    }
}