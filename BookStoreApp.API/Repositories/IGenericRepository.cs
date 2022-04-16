using BookStoreApp.API.Models;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;
 

namespace BookStoreApp.API.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> AddAsync(T entity);

        Task DeleteAsync(int id);
       
        Task<bool> Exists(int id);
        Task<List<T>> GetAllAsync();
        Task<VirtualizeResponse<TResult>> GetAllAsync<TResult>(QueryParameters queryParameters) where TResult:class ;
         
        Task<T> GetAsync(int? id); 

        Task UpdateAsync(T entity);
    }
}
