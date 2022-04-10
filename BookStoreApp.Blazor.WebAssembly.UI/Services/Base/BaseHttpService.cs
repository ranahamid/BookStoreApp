using Blazored.LocalStorage;
using System.Net.Http.Headers;

namespace BookStoreApp.Blazor.WebAssembly.UI.Services.Base
{
    public class BaseHttpService
    {
        private readonly IClient _client;
        protected readonly ILocalStorageService _localStorageService;
        public BaseHttpService(IClient client, ILocalStorageService localStorageService)
        {
            _client = client;
            _localStorageService = localStorageService;
        }

        protected Response<Guid> ConvertApiExceptions<Guid>(ApiException ex)
        {
            if (ex.StatusCode == 400)
            {
                return new Response<Guid>()
                { Message = "Validation errors have occured.", ValidationErrors = ex.Response, Success = false };
            }
            else if (ex.StatusCode == 404)
            {
                return new Response<Guid>()
                { Message = "The request item could not be found.", Success = false };
            }
            else
            {
                return new Response<Guid>()
                { Message = "Something went wrong, please try again.", Success = false };
            }
        }
        protected async Task GetBearerToken()
        { 
            var token =await _localStorageService.GetItemAsync<string>("token");
            if(token != null)
            { 
                _client.HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            } 
        } 
    }
}
