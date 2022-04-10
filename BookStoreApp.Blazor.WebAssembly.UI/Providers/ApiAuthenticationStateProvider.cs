using System.Security.Claims;
using Blazored.LocalStorage;
using System.IdentityModel.Tokens.Jwt ;
using Microsoft.AspNetCore.Components.Authorization;

namespace BookStoreApp.Blazor.WebAssembly.UI.Providers
{
    public class ApiAuthenticationStateProvider: AuthenticationStateProvider
    {
        private readonly ILocalStorageService _localStorageService;
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
        public ApiAuthenticationStateProvider(ILocalStorageService localStorageService)
        {
            _localStorageService=localStorageService;
            _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        }
        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity());
            var savedToken = await GetToken();
            if (string.IsNullOrEmpty(savedToken))
            {
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            } 
            var tokenContent = await GetSecurityToken(savedToken);
            if (tokenContent.ValidTo < DateTime.Now)
            {
                return new AuthenticationState(user);
            }

           
            user =await  GetUserPrincipal();
            return new AuthenticationState(user);
             
        }

        private async Task<string> GetToken()
        {
            var savedToken = await _localStorageService.GetItemAsync<string>("token");
            return savedToken;
        }
        private async Task<JwtSecurityToken> GetSecurityToken(string savedToken)
        {
            var tokenContent = _jwtSecurityTokenHandler.ReadJwtToken(savedToken);
            return tokenContent;
        }

        public async Task LoggedIn()
        {  
            var user =  await GetUserPrincipal(); 
            var authState = Task.FromResult(new AuthenticationState(user));
            NotifyAuthenticationStateChanged(authState);
        }

        public async Task<ClaimsPrincipal> GetUserPrincipal()
        {
            var claims = await GetClaims();
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims, "jwt"));
            return user;
        }
        public async Task LoggedOut()
        {
            await _localStorageService.RemoveItemAsync("token");
            var noBody = new ClaimsPrincipal(new ClaimsIdentity());
            var authState = Task.FromResult(new AuthenticationState(noBody));
            NotifyAuthenticationStateChanged(authState);
        }

        private async Task<List<Claim>> GetClaims()
        {
            var savedToken = await GetToken();
            var tokenContent = await GetSecurityToken(savedToken);
            var claims = tokenContent.Claims.ToList();
            //var tokenFullName = tokenContent.Claims.Where(x => x.Type == "fullName").Select(x => x.Value).FirstOrDefault();
            //if (string.IsNullOrEmpty(tokenFullName))
            //{
            //    tokenFullName = tokenContent.Subject;
            //}
            //claims.Add(new Claim(ClaimTypes.Name, tokenFullName));
            return claims;
        }


    }
}
