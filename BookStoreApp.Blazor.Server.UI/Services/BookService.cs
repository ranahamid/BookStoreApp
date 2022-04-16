using AutoMapper;
using Blazored.LocalStorage;
using BookStoreApp.Blazor.Server.UI.Models;
using BookStoreApp.Blazor.Server.UI.Services.Base;
namespace BookStoreApp.Blazor.Server.UI.Services
{

    public class BookService : BaseHttpService, IBookService
    {
        private readonly IClient _client;
        private readonly IMapper _mapper;

        public BookService(IClient client, ILocalStorageService localStorageService, IMapper mapper) : base(client, localStorageService)
        {
            _client = client;
            _mapper = mapper;
        }
        public async Task<Response<int>> Create(BookCreateDto Book)
        {
            Response<int> response = new Response<int>();
            try
            {
                await GetBearerToken();
                await _client.BooksPOSTAsync(Book);
            }
            catch (ApiException ex)
            {
                response = ConvertApiExceptions<int>(ex);
            }
            return response;
        }

        public async Task<Response<int>> Delete(int id)
        {
            Response<int> response = new Response<int>();
            try
            {
                await GetBearerToken();
                await _client.BooksDELETEAsync(id);
            }
            catch (ApiException ex)
            {
                response = ConvertApiExceptions<int>(ex);
            }
            return response;
        }

        public async Task<Response<int>> Edit(int id, BookUpdateDto Book)
        {
            Response<int> response = new();
            try
            {
                await GetBearerToken();
                await _client.BooksPUTAsync(id, Book);
            }
            catch (ApiException ex)
            {
                response = ConvertApiExceptions<int>(ex);
            }
            return response;
        }
        public async Task<Response<BookReadOnlyDtoVirtualizeResponse>> Get(QueryParameters queryParams)
        {
            Response<BookReadOnlyDtoVirtualizeResponse> response;

            try
            {
                await GetBearerToken();
                var data = await _client.BooksGETAsync(queryParams.StartIndex, queryParams.PageSize);
                response = new Response<BookReadOnlyDtoVirtualizeResponse>
                {
                    Data = data,
                    Success = true
                };
            }
            catch (ApiException exception)
            {
                response = ConvertApiExceptions<BookReadOnlyDtoVirtualizeResponse>(exception);
            }

            return response;
        }
        public async Task<Response<List<BookReadOnlyDto>>> Get()
        {
            var response = new Response<List<BookReadOnlyDto>>();
            try
            {
                await GetBearerToken();
                var data = await _client.BooksGetAllAsync();
                response = new Response<List<BookReadOnlyDto>>
                {
                    Data = data.ToList(),
                    Success = true
                };
            }
            catch (ApiException ex)
            {
                response = ConvertApiExceptions<List<BookReadOnlyDto>>(ex);
            }
            return response;
        }

        public async Task<Response<BookDetailsDto>> Get(int id)
        {
            Response<BookDetailsDto> response;
            try
            {
                await GetBearerToken();
                var data = await _client.BooksGET2Async(id);
                response = new Response<BookDetailsDto>
                {
                    Data = data,
                    Success = true
                };
            }
            catch (ApiException ex)
            {
                response = ConvertApiExceptions<BookDetailsDto>(ex);
            }
            return response;
        }

        public async Task<Response<BookUpdateDto>> GetForUpdate(int id)
        {
            Response<BookUpdateDto> response;
            try
            {
                await GetBearerToken();
                var data = await _client.BooksGET2Async(id);
                var mapResult = _mapper.Map<BookUpdateDto>(data);
                response = new Response<BookUpdateDto>
                {
                    Data = mapResult,
                    Success = true
                };
            }
            catch (ApiException ex)
            {
                response = ConvertApiExceptions<BookUpdateDto>(ex);
            }
            return response;
        }
    }
}
