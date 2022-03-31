using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using BookStoreApp.API.Data;
using BookStoreApp.API.Models.User;
using BookStoreApp.API.Static;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace BookStoreApp.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly IMapper _mapper;
        private readonly ILogger<AuthController> _logger;
        private readonly UserManager<ApiUser> _userManager;
        private readonly IConfiguration _configuration;

        public AuthController(IMapper mapper, ILogger<AuthController> logger, UserManager<ApiUser> userManager, IConfiguration configuration)
        {
            _mapper = mapper;
            _logger = logger;
            _userManager = userManager;
            _configuration = configuration;
        }

        [HttpPost]
        [Route("Register")]
        public async Task<IActionResult> Register(UserDto userDto)
        {
            _logger.LogInformation($"Registration attempt in {nameof(Register)} for {userDto.Email}");
            try
            {
                var user = _mapper.Map<ApiUser>(userDto);
                user.UserName = userDto.Email;
                var result = await _userManager.CreateAsync(user, userDto.Password);
                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(error.Code, error.Description);
                    }

                    return BadRequest(modelState: ModelState);
                }

                await _userManager.AddToRoleAsync(user, "User");
                return Accepted();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in performing Registration {nameof(Register)}");
                return Problem($"Something went wrong in the {nameof(Register)}", statusCode: 500);
            }
        }
        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login(LoginUserDto userDto)
        {
            _logger.LogInformation($"Login attempt in {nameof(Login)} for {userDto.Email}");
            try
            {

                var user = await _userManager.FindByEmailAsync(userDto.Email);
                var passwordValid = await _userManager.CheckPasswordAsync(user, userDto.Password);
                if (user == null || !passwordValid)
                {
                    return Unauthorized();
                }

                var token = await GenerateToken(user);
                var response = new AuthResponse
                {
                    Email = userDto.Email,
                    Token = token,
                    UserId = user.Id
                };
                return Accepted(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error in performing Login {nameof(Login)}");
                return Problem($"Something went wrong in the {nameof(Login)}", statusCode: 500);
            }
        }
        public async Task<string> GenerateToken(ApiUser user)
        {

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]));
            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var claims = await GetClaims(user);
            var token = GenerateTokenOptions(signingCredentials, claims);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<IEnumerable<Claim>> GetClaims(ApiUser user)
        {
            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(x => new Claim(ClaimTypes.Role, x)).ToList();

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim("fullName", user.FirstName + " " + user.LastName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Aud, _configuration["Jwt:Audience"]),
                new Claim(JwtRegisteredClaimNames.Iss, _configuration["Jwt:Issuer"]),
                new Claim(CustomClaimTypes.uid, user.Id),
            }.Union(roleClaims).Union(userClaims);
            return claims;
        }
        private JwtSecurityToken GenerateTokenOptions(SigningCredentials signingCredentials, IEnumerable<Claim> claims)
        {

            var token = new JwtSecurityToken();
            var life = _configuration["JwtSettings:Duration"];
            if (Int32.TryParse(life, out int expHrs))
            {
                var expiration = DateTime.Now.AddHours(expHrs);
                token = new JwtSecurityToken
                (
                    issuer: _configuration["Jwt:Issuer"],
                    audience: _configuration["Jwt:Audience"],
                    claims: claims,
                    expires: expiration,
                    signingCredentials: signingCredentials);
            }
            return token;
        }



    }
}
