using AggregateBy.Data;
using AggregateBy.Models;
using AggregateBy.Services;
using Microsoft.EntityFrameworkCore;

public class ProductReadService(AppDbContext context) : IProductReadService
{
    public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
    {
        return await context.Products
            .AsNoTracking()
            .Select(p => new ProductDTO
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                CategoryId = p.CategoryId
            })
            .ToListAsync();
    }

    public async Task<PagedProductResponseDTO> GetPagedProductsAsync(int pageSize, int? lastProductId = null)
    {
        var query = context.Products.AsNoTracking().AsQueryable();

        if (lastProductId.HasValue)
        {
            query = query.Where(p => p.Id > lastProductId.Value);
        }

        var pagedProducts = await query
            .OrderBy(p => p.Id)
            .Take(pageSize)
            .Select(p => new ProductDTO
            {
                Id = p.Id,
                Name = p.Name,
                Price = p.Price,
                CategoryId = p.CategoryId
            })
            .ToListAsync();

        var lastId = pagedProducts.LastOrDefault()?.Id;
        var hasNextPage = await context.Products.AnyAsync(p => p.Id > lastId);
        var averagePricePerCategory = await GetAveragePricePerCategoryAsync(pagedProducts);

        var result = new PagedProductResponseDTO
        {
            Items = pagedProducts,
            PageSize = pageSize,
            HasNextPage = hasNextPage,
            HasPreviousPage = lastProductId.HasValue,
            AveragePricePerCategory = averagePricePerCategory
        };

        return result;
    }

    private async Task<Dictionary<int, decimal>> GetAveragePricePerCategoryAsync(List<ProductDTO> products)
    {
        if (products == null || !products.Any())
        {
            return [];
        }

        var aggregateByTask = Task.Run(() =>
        {
            var aggregateBy = products.AggregateBy(
                product => product.CategoryId,
                x => (Sum: 0m, Count: 0),
                (acc, product) => (acc.Sum + product.Price, acc.Count + 1));

            var averagePriceByCategory = aggregateBy.ToDictionary(
                kvp => kvp.Key,
                kvp => Math.Round(kvp.Value.Sum / kvp.Value.Count, 2));

            return averagePriceByCategory;
        });
        return await aggregateByTask;
    }
}
