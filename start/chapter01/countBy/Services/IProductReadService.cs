using countBy.Models;
using CountBy.Models;
namespace CountBy.Services;

public interface IProductReadService {
    Task<IEnumerable<ProductDTO>> GetAllProductsAsync();
    Task<ProductDTO?> GetAProductAsync(int id);
    Task<IReadOnlyCollection<CategoryDTO>> GetCategoryInfoAsync();
}
