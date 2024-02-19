using WebRozetka.Data.Entities.Category;
using WebRozetka.Data.Entities.Photo;

namespace WebRozetka.Interfaces.Repo
{
    public interface IPhotoRepository : IRepository<PhotoEntity>
    {
        IQueryable<PhotoEntity> GetAllByProduct(int productId);
    }
}
