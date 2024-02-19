using WebRozetka.Data.Entities.Product;

namespace WebRozetka.Interfaces.Repo
{
    public interface IProductRepository : IRepository<ProductEntity>
    {
        IQueryable<ProductEntity> GetByCategory(int category);
    }
}
