using WebRozetka.Models;

namespace WebRozetka.Interfaces.Repo
{
    public interface IRepository<T>
    {
        Task<T> GetByIdAsync(int id);
        IQueryable<T> GetAll();
        IQueryable<T> GetAll(QueryParameters queryParameters);
        Task<int> GetCountAsync(QueryParameters queryParameters);
        T AddAsync(T entity);
        T Update(T entity);
        void DeleteAsync(int id);
        Task<bool> Save();
    }

}
