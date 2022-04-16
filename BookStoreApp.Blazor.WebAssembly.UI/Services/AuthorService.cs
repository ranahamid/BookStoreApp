using AutoMapper;
using Blazored.LocalStorage;
using BookStoreApp.Blazor.WebAssembly.UI.Models;
using BookStoreApp.Blazor.WebAssembly.UI.Services.Base;

namespace BookStoreApp.Blazor.WebAssembly.UI.Services
{
    public class AuthorService : BaseHttpService, IAuthorService
    {
        private readonly IClient _client;
        private readonly IMapper _mapper;

        public AuthorService(IClient client, ILocalStorageService localStorageService, IMapper mapper):base(client, localStorageService)
        {
            _client = client;
            _mapper = mapper;
        }
        public async Task<Response<int>> Create(AuthorCreateDto author)
        {
           Response<int> response = new Response<int>();
            try
            {
                await GetBearerToken();
                await _client.AuthorsPOSTAsync(author);
            }
            catch (ApiException ex)
            {
                response= ConvertApiExceptions<int>(ex);
            }
            return response;
        }

        public async Task<Response<int>> Delete(int id)
        {
            Response<int> response = new Response<int>();
            try
            {
                await GetBearerToken();
                await _client.AuthorsDELETEAsync(id);
            }
            catch (ApiException ex)
            {
                response = ConvertApiExceptions<int>(ex);
            }
            return response;
        }

        public async Task<Response<int>> Edit(int id, AuthorUpdateDto author)
        {
            Response<int> response = new();
            try
            {
                await GetBearerToken();
                await _client.AuthorsPUTAsync(id, author);
            }
            catch (ApiException ex)
            {
                response = ConvertApiExceptions<int>(ex);
            }
            return response;
        }
        public async Task<Response<AuthorReadOnlyDtoVirtualizeResponse>> Get(QueryParameters queryParams)
        {
            Response<AuthorReadOnlyDtoVirtualizeResponse> response;

            try
            {
                await GetBearerToken();
                var data = await _client.AuthorsGETAsync(queryParams.StartIndex, queryParams.PageSize);
                response = new Response<AuthorReadOnlyDtoVirtualizeResponse>
                {
                    Data = data,
                    Success = true
                };
            }
            catch (ApiException exception)
            {
                response = ConvertApiExceptions<AuthorReadOnlyDtoVirtualizeResponse>(exception);
            }

            return response;
        }
        public async  Task<Response<List<AuthorReadOnlyDto>>> Get()
        {
            var response = new Response<List<AuthorReadOnlyDto>>();
            try
            {
                await GetBearerToken();
                var data = await _client.AuthorsGetAllAsync();
                response = new Response<List<AuthorReadOnlyDto>>
                {
                    Data = data.ToList(),
                    Success = true
                };
            }
            catch (ApiException ex)
            {
                response = ConvertApiExceptions<List<AuthorReadOnlyDto>>(ex);
            }
            return response;
        }

        public async  Task<Response<AuthorDetailsDto>> Get(int id)
        {
            Response<AuthorDetailsDto> response;
            try
            {
                await GetBearerToken();
                var data= await _client.AuthorsGET2Async(id);
                response = new Response<AuthorDetailsDto>
                {
                    Data = data,
                    Success = true
                };
            }
            catch (ApiException ex)
            {
                response = ConvertApiExceptions<AuthorDetailsDto>(ex);
            }
            return response;
        }

        public async  Task<Response<AuthorUpdateDto>> GetForUpdate(int id)
        { 
            Response<AuthorUpdateDto> response ;
            try
            {
                await GetBearerToken();
                var data = await _client.AuthorsGET2Async(id);
                var mapResult = _mapper.Map<AuthorUpdateDto>(data); 
                response = new Response<AuthorUpdateDto>
                {
                    Data = mapResult,
                    Success = true
                };
            }
            catch (ApiException ex)
            {
                response = ConvertApiExceptions<AuthorUpdateDto>(ex);
            }
            return response;
        }
    } 
}
