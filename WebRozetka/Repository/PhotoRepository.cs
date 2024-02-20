using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using WebRozetka.Data;
using WebRozetka.Data.Entities.Category;
using WebRozetka.Data.Entities.Photo;
using WebRozetka.Interfaces.Repo;
using WebRozetka.Models;

namespace WebRozetka.Repository
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly AppEFContext _context;

        public PhotoRepository(AppEFContext context)
        {
            _context = context;
        }

        public PhotoEntity AddAsync(PhotoEntity entity)
        {
            _context.Set<PhotoEntity>().Add(entity);
            return entity;
        }

        public void DeleteAsync(int id)
        {
            var photo = _context.Set<PhotoEntity>().Find(id);

            if (photo != null)
            {
                _context.Set<PhotoEntity>().Remove(photo);
            }
        }

        public IQueryable<PhotoEntity> GetAll()
        {
            throw new NotImplementedException();
        }

        public IQueryable<PhotoEntity> GetAll(QueryParameters queryParameters)
        {
            throw new NotImplementedException();
        }

        public IQueryable<PhotoEntity> GetAllByProduct(int productId)
        {
            return _context.Set<PhotoEntity>().Where(x => x.ProductId == productId);
        }

        public async Task<PhotoEntity> GetByIdAsync(int id)
        {
            return await _context.Set<PhotoEntity>().Where(x => x.Id == id).SingleOrDefaultAsync();
        }

        public Task<int> GetCountAsync(QueryParameters queryParameters)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Save()
        {
            return (await _context.SaveChangesAsync() >= 0);
        }

        public PhotoEntity Update(PhotoEntity entity)
        {
            throw new NotImplementedException();
        }


    }
}
