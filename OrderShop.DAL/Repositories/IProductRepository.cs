using OrderShop.Core.Entities;

namespace OrderShop.DAL.Repositories;

public interface IProductRepository : IRepository<Product>
{
    IEnumerable<Product> GetByCategory(int categoryId);
    IEnumerable<Product> GetByPriceRange(decimal minPrice, decimal maxPrice);
    IEnumerable<Product> SearchByName(string namePart);
    IEnumerable<Product> GetOutOfStock();
}