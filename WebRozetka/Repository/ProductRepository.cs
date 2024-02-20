using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
            IQueryable<ProductEntity> entities = _context.Set<ProductEntity>()
                .Include(x => x.Photos)
                .Where(x => !x.IsDeleted);

            if (!string.IsNullOrEmpty(queryParameters.Query))
            {
                entities = entities.Where(x => x.Name.ToLower().Contains(queryParameters.Query.ToLower()));
            }

            if (queryParameters.CategoryId > 0)
            {
                entities = entities.Where(x => x.CategoryId == queryParameters.CategoryId);
            }

            if (queryParameters.PriceMin > 0)
            {
                entities = entities.Where(x => x.Price >= queryParameters.PriceMin);
            }
            if (queryParameters.PriceMax > 0)
            {
                entities = entities.Where(x => x.Price <= queryParameters.PriceMax);
            }

            if (queryParameters.QuantityMin > 0)
            {
                entities = entities.Where(x => x.Quantity >= queryParameters.QuantityMin);
            }
            if (queryParameters.QuantityMax > 0)
            {
                entities = entities.Where(x => x.Quantity <= queryParameters.QuantityMax);
            }

            entities = entities.OrderBy(x => x.Id);

            entities = entities
                .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
                .Take(queryParameters.PageCount);

            return entities;
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

        public Task<int> GetCountAsync(QueryParameters queryParameters)
        {
            IQueryable<ProductEntity> entities = _context.Set<ProductEntity>()
                 .Where(x => !x.IsDeleted);

            if (!string.IsNullOrEmpty(queryParameters.Query))
            {
                entities = entities.Where(x => x.Name.ToLower().Contains(queryParameters.Query.ToLower()));
            }

            if (queryParameters.CategoryId > 0)
            {
                entities = entities.Where(x => x.CategoryId == queryParameters.CategoryId);
            }

            if (queryParameters.PriceMin > 0)
            {
                entities = entities.Where(x => x.Price >= queryParameters.PriceMin);
            }
            if (queryParameters.PriceMax > 0)
            {
                entities = entities.Where(x => x.Price <= queryParameters.PriceMax);
            }

            if (queryParameters.QuantityMin > 0)
            {
                entities = entities.Where(x => x.Quantity >= queryParameters.QuantityMin);
            }
            if (queryParameters.QuantityMax > 0)
            {
                entities = entities.Where(x => x.Quantity <= queryParameters.QuantityMax);
            }

            return entities.CountAsync();
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
