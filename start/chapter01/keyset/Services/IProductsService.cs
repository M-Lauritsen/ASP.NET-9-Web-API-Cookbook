using cookbook.Models;
using keyset.Models;
namespace cookbook.Services;

public interface IProductsService {
    Task<IEnumerable<ProductDTO>> GetAllProductsAsync();
    Task<PagedProdectResponseDTO> GetPagedProductsAsync(int pageSize, int? lastProductId = null);
}