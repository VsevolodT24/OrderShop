using OrderShop.Core.Entities;

namespace OrderShop.DAL.Repositories;

public interface IPromoCodeRepository : IGenericRepository<PromoCode>
{
    PromoCode? GetByCode(string code);
}

public class PromoCodeRepository : GenericRepository<PromoCode>, IPromoCodeRepository
{
    public PromoCodeRepository(OrderShopDbContext context) : base(context) { }

    public PromoCode? GetByCode(string code) => Set.FirstOrDefault(p => p.Code == code);
}