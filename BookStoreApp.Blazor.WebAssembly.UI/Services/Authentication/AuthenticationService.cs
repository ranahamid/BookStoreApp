using Blazored.LocalStorage;
using BookStoreApp.Blazor.WebAssembly.UI.Providers;
using BookStoreApp.Blazor.WebAssembly.UI.Services.Base;
using Microsoft.AspNetCore.Components.Authorization;

namespace BookStoreApp.Blazor.WebAssembly.UI.Services.Authentication
{
    public class AuthenticationService: IAuthenticationService
    {
        private readonly IClient _httpClient;
        private readonly ILocalStorageService _localStorageService;
        private readonly AuthenticationStateProvider _authenticationStateProvider;
        public AuthenticationService(IClient httpClient, ILocalStorageService localStorageService, AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClient = httpClient;
            _localStorageService = localStorageService;
            _authenticationStateProvider = authenticationStateProvider;
        }
        public async Task<bool> AuthenticateAsync(LoginUserDto loginUserDto)
        {
             var response =await  _httpClient.LoginAsync(loginUserDto);

            //store token
            await _localStorageService.SetItemAsync("token", response.Token);
            //change auth state of app
            await ((ApiAuthenticationStateProvider)_authenticationStateProvider).LoggedIn();
            return true;
        }

        public async Task Logout()
        {
            await ((ApiAuthenticationStateProvider)_authenticationStateProvider).LoggedOut();
        }

    }
}
