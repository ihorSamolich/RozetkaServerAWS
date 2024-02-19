using Microsoft.EntityFrameworkCore;
using WebRozetka.Data;
using WebRozetka.Data.Entities.Category;
using WebRozetka.Data.Entities.Product;
using WebRozetka.Interfaces.Repo;
using WebRozetka.Models;

namespace WebRozetka.Repository
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppEFContext _context;

        public ProductRepository(AppEFContext context)
        {
            _context = context;
        }

        public ProductEntity AddAsync(ProductEntity entity)
        {
            _context.Set<ProductEntity>().Add(entity);
            return entity;
        }

        public void DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<ProductEntity> GetAll()
        {
            return _context.Set<ProductEntity>()
                 .Include(x => x.Photos)
                 .Where(x => !x.IsDeleted);
        }

        public IQueryable<ProductEntity> GetAll(QueryParameters queryParameters)
        {
            throw new NotImplementedException();
        }

        public IQueryable<ProductEntity> GetByCategory(int category)
        {
            return _context.Set<ProductEntity>()
               .Include(x => x.Photos)
               .Where(x => !x.IsDeleted && x.CategoryId == category);
        }

        public async Task<ProductEntity> GetByIdAsync(int id)
        {
            return await _context.Set<ProductEntity>()
                .Include(x => x.Photos.OrderBy(p => p.Priority))
                .Where(x => !x.IsDeleted && x.Id == id)
                .SingleOrDefaultAsync();
        }

        public Task<int> GetCountAsync(string search = "")
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Save()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }

        public ProductEntity Update(ProductEntity entity)
        {
            _context.Set<ProductEntity>().Update(entity);
            return entity;
        }
    }
}
