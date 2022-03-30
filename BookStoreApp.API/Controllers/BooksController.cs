#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStoreApp.API.Data;
using BookStoreApp.API.Models.Book;
using BookStoreApp.API.Static;

namespace BookStoreApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BookStoreDbContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger<BooksController> _logger;

        public BooksController(BookStoreDbContext context, IMapper mapper, ILogger<BooksController> logger)
        {
            _context = context;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookReadOnlyDto>>> GetBooks()
        {
             
            try
            {
                var books = await _context.Books.Include(x=>x.Author).ToListAsync();
                var bookDtos = _mapper.Map<IEnumerable<BookReadOnlyDto>>(books);
                return Ok(bookDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in performing GET {nameof(GetBooks)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookReadOnlyDto>> GetBook(int id)
        {
            try
            {
                var book = await _context.Books.Include(x => x.Author).FirstOrDefaultAsync(x=>x.Id== id);
                if (book == null)
                {
                    _logger.LogWarning($"Record not found in {nameof(GetBook)}- Id: {id}");
                    return NotFound();
                }
                var bookDto = _mapper.Map<BookReadOnlyDto>(book);
                return Ok(bookDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in performing GET {nameof(GetBook)}");
                return StatusCode(500, Messages.Error500Message);
            }
             
        }

        // PUT: api/Books/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBook(int id, BookUpdateDto bookDto)
        {

            if (id != bookDto.Id)
            {
                _logger.LogWarning($"Update invalid in {nameof(PutBook)}- Id: {id}");
                return BadRequest();
            }
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                _logger.LogWarning($"Record not found in {nameof(GetBook)}- Id: {id}");
                return NotFound();
            }
            _mapper.Map(bookDto, book);
            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, $"Error in performing PUT {nameof(PutBook)}");
                    return StatusCode(500, Messages.Error500Message);
                }
            }

            return NoContent();
        }

        // POST: api/Books
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Book>> PostBook(BookCreateDto bookDto)
        {
            try
            {
                var book = _mapper.Map<Book>(bookDto);
                await _context.Books.AddAsync(book);
                await _context.SaveChangesAsync();

                return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in performing POST {nameof(PostBook)}");
                return StatusCode(500, Messages.Error500Message);
            } 
        }

        // DELETE: api/Books/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                var book = await _context.Books.FindAsync(id);
                if (book == null)
                {
                    _logger.LogWarning($"Record not found in {nameof(DeleteBook)}- Id: {id}");
                    return NotFound();
                }
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in performing DELETE {nameof(DeleteBook)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}
