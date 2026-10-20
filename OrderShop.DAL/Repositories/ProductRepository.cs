using OrderShop.Core.Entities;

namespace OrderShop.DAL.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly OrderShopDbContext _context;

    public ProductRepository(OrderShopDbContext context) => _context = context;

    public Product? GetById(int id) => _context.Products.Find(id);
    public IEnumerable<Product> GetAll() => _context.Products.ToList();
    public void Add(Product entity) => _context.Products.Add(entity);
    public void Update(Product entity) => _context.Products.Update(entity);
    public void Delete(int id)
    {
        Product? product = _context.Products.Find(id);
        if (product != null) _context.Products.Remove(product);
    }

    public IEnumerable<Product> GetByCategory(int categoryId) => 
        _context.Products.Where(p => p.CategoryId == categoryId).ToList();

    public IEnumerable<Product> GetByPriceRange(decimal minPrice, decimal maxPrice) => 
        _context.Products.AsEnumerable()
            .Where(p => p.Price >= minPrice && p.Price <= maxPrice)
            .OrderBy(p => p.Price)
            .ToList();

    public IEnumerable<Product> SearchByName(string namePart) => 
        _context.Products.Where(p => p.Name.Contains(namePart)).ToList();

    public IEnumerable<Product> GetOutOfStock() => 
        _context.Products.Where(p => p.StockQuantity == 0).ToList();
}