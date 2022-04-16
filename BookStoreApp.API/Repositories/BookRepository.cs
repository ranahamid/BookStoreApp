using AutoMapper;
using AutoMapper.QueryableExtensions;
using BookStoreApp.API.Data;
using BookStoreApp.API.Models.Book;
using Microsoft.EntityFrameworkCore;

namespace BookStoreApp.API.Repositories
{
    public class BookRepository : GenericRepository<Book>, IBookRepository
    {
        private readonly BookStoreDbContext _context;
        private readonly IMapper _mapper;
        public BookRepository(BookStoreDbContext context, IMapper mapper):base(context, mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async  Task<List<BookReadOnlyDto>> GetAllBooksAsync()
        {
            var bookDtos = await _context.Books.Include(x => x.Author).ProjectTo<BookReadOnlyDto>(_mapper.ConfigurationProvider).ToListAsync();
            return bookDtos;
        }

        public async  Task<BookDetailsDto> GetBookAsync(int id)
        {
            var book = await _context.Books.Include(x => x.Author).FirstOrDefaultAsync(x => x.Id == id);
            var bookDto = _mapper.Map<BookDetailsDto>(book);
            return bookDto;
        }
    }
}
