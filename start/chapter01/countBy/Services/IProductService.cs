using countBy.Models;
using CountBy.Models;

namespace countBy.Services;

public interface IProductService
{
    Task<IEnumerable<ProductDTO>> GetAllProductsAsync();
    Task<IReadOnlyCollection<CategoryDTO>> GetCategoryInfoAsync();
}
