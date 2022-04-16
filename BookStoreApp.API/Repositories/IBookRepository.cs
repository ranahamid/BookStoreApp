using BookStoreApp.API.Data;
using BookStoreApp.API.Models.Book;

namespace BookStoreApp.API.Repositories
{
    public interface IBookRepository:IGenericRepository<Book>
    {
        Task<BookDetailsDto> GetBookAsync(int id);
        Task<List<BookReadOnlyDto>> GetAllBooksAsync();
    }
}
