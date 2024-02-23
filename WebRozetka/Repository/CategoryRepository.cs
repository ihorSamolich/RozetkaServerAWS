using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using WebRozetka.Data;
using WebRozetka.Data.Entities.Category;
using WebRozetka.Interfaces.Repo;
using WebRozetka.Models;

namespace WebRozetka.Repository
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppEFContext _context;

        public CategoryRepository(AppEFContext context)
        {
            _context = context;
        }

        public async Task<CategoryEntity> GetByIdAsync(int id)
        {
            return await _context.Set<CategoryEntity>().Where(x => !x.IsDeleted && x.Id == id).FirstOrDefaultAsync();
        }

        public IQueryable<CategoryEntity> GetAll()
        {
            return _context.Set<CategoryEntity>().Where(x => !x.IsDeleted);
        }
        public IQueryable<CategoryEntity> GetAll(QueryParameters queryParameters)
        {
            IQueryable<CategoryEntity> entities = _context.Set<CategoryEntity>().Where(x => !x.IsDeleted).OrderBy(x => x.Id);

            if (!queryParameters.Query.IsNullOrEmpty())
            {
                entities = entities.Where(x => x.Name.ToLower().Contains(queryParameters.Query.ToLower()));
            }

            return entities
                .Skip(queryParameters.PageCount * (queryParameters.Page - 1))
                .Take(queryParameters.PageCount);
        }

        public CategoryEntity AddAsync(CategoryEntity entity)
        {
            _context.Set<CategoryEntity>().Add(entity);
            return entity;
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await GetByIdAsync(id);

            if (entity != null)
            {
                entity.IsDeleted = true;
            }
        }

        public CategoryEntity Update(CategoryEntity entity)
        {
            _context.Set<CategoryEntity>().Update(entity);
            return entity;
        }

        public Task<int> GetCountAsync(QueryParameters queryParameters)
        {
            var entities = _context.Set<CategoryEntity>().Where(x => !x.IsDeleted);

            if (!queryParameters.Query.IsNullOrEmpty())
            {
                entities = entities.Where(x => x.Name.ToLower().Contains(queryParameters.Query.ToLower()));
            }

            return entities.CountAsync();
        }

        public async Task<bool> Save()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}
