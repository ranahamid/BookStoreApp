﻿#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookStoreApp.API.Data;
using BookStoreApp.API.Models.Book;
using BookStoreApp.API.Static;
using Microsoft.AspNetCore.Authorization;
using BookStoreApp.API.Repositories;
using BookStoreApp.API.Models;

namespace BookStoreApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [Authorize]
    public class BooksController : ControllerBase
    {
        private readonly IBookRepository _bookRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<BooksController> _logger; 
        private readonly IWebHostEnvironment _webHostEnvironment;  
        public BooksController(IBookRepository bookRepository, IMapper mapper, ILogger<BooksController> logger, IWebHostEnvironment webHostEnvironment
           )
        {
            _bookRepository = bookRepository;
            _mapper = mapper;
            _logger = logger;
            _webHostEnvironment = webHostEnvironment;
          
        }

        // GET: api/Books
        [HttpGet]
        public async Task<ActionResult<VirtualizeResponse<BookReadOnlyDto>>> GetBooks([FromQuery] QueryParameters queryParameters)
        { 
            try
            { 
                var bookDtos = await _bookRepository.GetAllAsync<BookReadOnlyDto>(queryParameters);
                return Ok(bookDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in performing GET {nameof(GetBooks)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }
        [HttpGet("BooksGetAll")]
        public async Task<ActionResult<IEnumerable<BookReadOnlyDto>>> GetAllBooks()
        {
            var bookDtos = await _bookRepository.GetAllBooksAsync();
            return Ok(bookDtos);
        }

        // GET: api/Books/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BookDetailsDto>> GetBook(int id)
        {
            try
            {
                var bookDto = await _bookRepository.GetAsync(id);
                if (bookDto == null)
                {
                    _logger.LogWarning($"Record not found in {nameof(GetBook)}- Id: {id}");
                    return NotFound();
                }
               
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
       // [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> PutBook(int id, BookUpdateDto bookDto)
        {

            if (id != bookDto.Id)
            {
                _logger.LogWarning($"Update invalid in {nameof(PutBook)}- Id: {id}");
                return BadRequest();
            }
          
            var book = await _bookRepository.GetAsync(id);
            if (book == null)
            {
                _logger.LogWarning($"Record not found in {nameof(GetBook)}- Id: {id}");
                return NotFound();
            }
            if (!string.IsNullOrEmpty(bookDto.ImageData))
            {
                bookDto.Image = CreateFile(bookDto.ImageData, bookDto.OriginalImageName);
                //deleet existing file
                var picName = Path.GetFileName(book.Image);
              
                string rootPath = _webHostEnvironment.WebRootPath;
                //string contentRootPath = _webHostEnvironment.ContentRootPath;
               // var rootPath = webRootPath  + contentRootPath;

                //var folderpath = $"{rootPath}\\content\\images\\books";

                //if (!System.IO.File.Exists(folderpath))
                //{
                //    System.IO.Directory.CreateDirectory(folderpath);
                //}
                //var path = $"folderpath\\{fileName}";


                var path = $"{rootPath}\\content\\images\\books\\{picName}";
              
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }
             _mapper.Map(bookDto, book); 
            try
            {
                await _bookRepository.UpdateAsync(book);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (! await BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, $"Error in performing PUT {nameof(PutBook)}");
                    return StatusCode(500, Messages.Error500Message);
                }
            }

            return Ok();
        }

        // POST: api/Books
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
       // [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<Book>> PostBook(BookCreateDto bookDto)
        {
            try
            {
                var book = _mapper.Map<Book>(bookDto);
                if (!string.IsNullOrEmpty(bookDto.ImageData))
                {
                    book.Image = CreateFile(bookDto.ImageData, bookDto.OriginalImageName);
                }
                await _bookRepository.AddAsync(book); 
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
     //   [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteBook(int id)
        {
            try
            {
                var book = await _bookRepository.GetAsync(id);
                if (book == null)
                {
                    _logger.LogWarning($"Record not found in {nameof(DeleteBook)}- Id: {id}");
                    return NotFound();
                }
                await _bookRepository.DeleteAsync(id);
                
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in performing DELETE {nameof(DeleteBook)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }
        private string CreateFile(string imageBase64Data, string imageName)
        {
            if( !string.IsNullOrEmpty(imageBase64Data) && !string.IsNullOrEmpty(imageName))
            {
                var url = HttpContext.Request.Host.Value;
                var extension = Path.GetExtension(imageName);
                var fileName = $"{Guid.NewGuid().ToString()}{extension}";

                string rootPath = _webHostEnvironment.WebRootPath;
                //string contentRootPath = _webHostEnvironment.ContentRootPath;
                //var rootPath = webRootPath + contentRootPath;

                var folderpath = $"{rootPath}\\content\\images\\books";
              
                if (!System.IO.File.Exists(folderpath))
                {
                    System.IO.Directory.CreateDirectory(folderpath);
                }
                var path = $"{folderpath}\\{fileName}";

                byte[] image = Convert.FromBase64String(imageBase64Data);
                var fileStream = System.IO.File.Create(path);
                fileStream.Write(image, 0, image.Length);
                fileStream.Close();
                return $"https://{url}/content/images/books/{fileName}";
            }
            return string.Empty;
        }
        private async Task< bool> BookExists(int id)
        {
            return await _bookRepository.Exists(id);
        }
    }
}
