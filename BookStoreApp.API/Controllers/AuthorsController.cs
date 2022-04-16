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
using BookStoreApp.API.Models.Author;
using BookStoreApp.API.Static;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using BookStoreApp.API.Repositories;
using BookStoreApp.API.Models;

namespace BookStoreApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
  //  [Authorize]
    public class AuthorsController : ControllerBase
    {
        private readonly IAuthorsRepository _authorsRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthorsController> _logger;

        public AuthorsController(IAuthorsRepository authorsRepository, IMapper mapper, ILogger<AuthorsController>  logger)
        {
            _authorsRepository = authorsRepository;
            _mapper = mapper;
            _logger = logger;
        }

        // GET: api/Authors
        [HttpGet]
        public async Task<ActionResult<VirtualizeResponse<AuthorReadOnlyDto>>> GetAuthors([FromQuery] QueryParameters queryParameters)
        {
            try
            {
                var authors = await _authorsRepository.GetAllAsync<AuthorReadOnlyDto>(queryParameters);
               // var authorDtos = _mapper.Map<IEnumerable<AuthorReadOnlyDto>>(authors);
                return Ok(authors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,$"Error in performing GET {nameof(GetAuthors)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        // GET: api/Authors/GetAll
        [HttpGet("AuthorsGetAll")]
        public async Task<ActionResult<List<AuthorReadOnlyDto>>> GetAuthors()
        {
            try
            {
                var authors = await _authorsRepository.GetAllAsync();
                var authorDtos = _mapper.Map<IEnumerable<AuthorReadOnlyDto>>(authors);
                return Ok(authorDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error Performing GET in {nameof(GetAuthors)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        // GET: api/Authors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AuthorDetailsDto>> GetAuthor(int id)
        {
            try
            {
                //var author = _context.Authors.Include(x=>x.Books).ProjectTo<AuthorDetailsDto>(_mapper.ConfigurationProvider).FirstOrDefault(x=>x.Id==id); 
                var author = await _authorsRepository.GetAuthorDetailsAsync(id);

                if (author == null)
                {
                    _logger.LogWarning($"Record not found in {nameof(GetAuthor)}- Id: {id}");
                    return NotFound();
                } 
                return Ok(author);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in performing GET {nameof(GetAuthor)}");
                return StatusCode(500, Messages.Error500Message);
            }
          
        }

        // PUT: api/Authors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        //[Authorize(Roles = "Administrator")]
        public async Task<IActionResult> PutAuthor(int id, AuthorUpdateDto authorDto)
        {
            if (id != authorDto.Id)
            {
                _logger.LogWarning($"Update invalid in {nameof(PutAuthor)}- Id: {id}");
                return BadRequest();
            }
            var author = await _authorsRepository.GetAsync(id);
            if (author == null)
            {
                _logger.LogWarning($"Record not found in {nameof(PutAuthor)}- Id: {id}");
                return NotFound();
            }

            _mapper.Map(authorDto,author);
           

            try
            {
                await _authorsRepository.UpdateAsync(author);                  
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!await AuthorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError(ex, $"Error in performing PUT {nameof(PutAuthor)}");
                    return StatusCode(500, Messages.Error500Message);
                }
            }

            return Ok();
        }

        // POST: api/Authors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
       // [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<Author>> PostAuthor(AuthorCreateDto authorDto)
        {
            try
            { 
                var author = _mapper.Map<Author>(authorDto);
                await _authorsRepository.AddAsync(author); 
                return CreatedAtAction(nameof(GetAuthor), new { id = author.Id }, author);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in performing POST {nameof(PostAuthor)}");
                return StatusCode(500, Messages.Error500Message);
            } 
        }

        // DELETE: api/Authors/5
        [HttpDelete("{id}")]
        //[Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            try
            {
                var author = await _authorsRepository.GetAsync(id);
                
                if (author == null)
                {
                    _logger.LogWarning($"Record not found in {nameof(DeleteAuthor)}- Id: {id}");
                    return NotFound();
                }

              await _authorsRepository.DeleteAsync(id); 
                return Ok();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in performing DELETE {nameof(DeleteAuthor)}");
                return StatusCode(500, Messages.Error500Message);
            }
        }

        private async Task<bool> AuthorExists(int id)
        {
            return await _authorsRepository. Exists(id);
        }
    }
}
