using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using PizzaMauiApp.API.DbStore;
using PizzaMauiApp.API.Models;

namespace PizzaMauiApp.API.SeedData;

public class StoreDbContextSeed
{
    public static async Task SeedAsync(ApplicationDbContext? context, ILoggerFactory loggerFactory)
    {
        if (context is null) return;

        var logger = loggerFactory?.CreateLogger<StoreDbContextSeed>();

        try
        {
            if (!context.PizzaProducts.Any())
            {
                string pathToPizzaItemsJson = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"SeedData/items.json");
                Trace.WriteLine(pathToPizzaItemsJson);
                var pizzaItemsFs = File.OpenRead(pathToPizzaItemsJson);
                var pizzaItems = await JsonSerializer.DeserializeAsync<List<PizzaProduct>>(pizzaItemsFs);
                foreach (var pizzaItem in pizzaItems) 
                {
                    context.PizzaProducts.Add(pizzaItem);
                }

                await context.SaveChangesAsync();
            }
        }
        catch (Exception e)
        {
            logger?.LogError(e.Message);
        }
    }
}