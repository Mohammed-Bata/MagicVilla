using AutoMapper;
using MagicVillaApi.Migrations;
using MagicVillaApi.Models;
using MagicVillaApi.Models.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MagicVillaApi.Data
{
    public class ApplicationUserDAO
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _RoleManager;
        private readonly IMapper _mapper;
        private string SecretKey;
        public ApplicationUserDAO(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, IConfiguration configuration, IMapper mapper)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _RoleManager = roleManager;
            _mapper = mapper;
            SecretKey = configuration.GetValue<string>("ApiSettings:Secret");
        }
        public bool IsUnique(string username)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.UserName == username);
            if (user == null)
            {
                return true;
            }
            return false;
        }
        public async Task<TokenDto> Login(LoginRequestDto dto)
        {
            var User = _dbContext.Users.FirstOrDefault(u => u.UserName.ToLower() == dto.UserName.ToLower());
            bool isvalid = await _userManager.CheckPasswordAsync(User, dto.Password);

            if (User == null || !isvalid)
            {
                return new TokenDto()
                {
                    AccessToken = "",
                };
            }
            var jwtTokenId = $"jti{Guid.NewGuid()}";
            var accessToken = await GetAccessToken(User, jwtTokenId);
            var refreshToken = await CreateNewRefreshToken(User.Id, jwtTokenId);

            TokenDto tokenDto = new TokenDto()
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };
            return tokenDto;
        }
        public async Task<UserDto> Register(RegisterationRequestDto dto)
        {
            ApplicationUser user = new()
            {
                UserName = dto.UserName,
                Email = dto.UserName.ToUpper(),
                Name = dto.Name,
            };

            var result = await _userManager.CreateAsync(user, dto.Password);
            if (result.Succeeded)
            {
                if (!_RoleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult())
                {
                    await _RoleManager.CreateAsync(new IdentityRole("Admin"));
                    await _RoleManager.CreateAsync(new IdentityRole("Client"));
                }
                await _userManager.AddToRoleAsync(user, dto.Role);
                var userToReturn = _dbContext.Users
                    .FirstOrDefault(u => u.UserName == dto.UserName);
                return _mapper.Map<UserDto>(userToReturn);
            }
            return new UserDto();
        }
        private async Task<string> GetAccessToken(ApplicationUser user, string jwtTokenId)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(SecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                    new Claim(JwtRegisteredClaimNames.Jti, jwtTokenId),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                    new Claim(JwtRegisteredClaimNames.Aud, "MagicVillaWeb")
                }),
                Expires = DateTime.UtcNow.AddMinutes(60),
                Issuer = "MagicVillaApi",
                Audience = "test-Magic",
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenStr = tokenHandler.WriteToken(token);
            return tokenStr;
        }
        private bool GetAccessTokenData(string accessToken,string expectedUserId,string expectedTokenId)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwt = tokenHandler.ReadJwtToken(accessToken);
                var jwtTokenId = jwt.Claims.FirstOrDefault(u=>u.Type == JwtRegisteredClaimNames.Jti).Value;
                var userId = jwt.Claims.FirstOrDefault(u=>u.Type == JwtRegisteredClaimNames.Sub).Value;
                return userId == expectedUserId && expectedTokenId == jwtTokenId; 
            }
            catch
            {
                return false;
            }
        }
        private async Task<string> CreateNewRefreshToken(string userId,string tokenId)
        {
            RefreshToken refreshToken = new()
            {
                IsValid = true,
                UserId = userId,
                JwtTokenId = tokenId,
                ExpiresAt = DateTime.UtcNow.AddDays(30),
                Refresh_Token = Guid.NewGuid() + "-" + Guid.NewGuid(),
            };
            await _dbContext.RefreshTokens.AddAsync(refreshToken);
            await _dbContext.SaveChangesAsync();
            return refreshToken.Refresh_Token;
        }
        public async Task<TokenDto> RefreshAccessToken(TokenDto tokenDto)
        {
            //find an existing refresh token
            var existingRefreshToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(t => t.Refresh_Token == tokenDto.RefreshToken);
            if (existingRefreshToken == null)
            {
                return new TokenDto();
            }

            //compare data from existing refresh token and access token provided and if there is any mismatch then consider it as fraud
            var isTokenValid = GetAccessTokenData(tokenDto.AccessToken,existingRefreshToken.UserId,existingRefreshToken.JwtTokenId);
            if (!isTokenValid)
            {
                await MarkTokenAsInvalid(existingRefreshToken);
                return new TokenDto();
            }

            //when someone tries to use not valid refresh token, fraud possible
            if (!existingRefreshToken.IsValid)
            {
                await MarkAllTokenInChainAsInvalid(existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);
            }
            //if just expired then mark as invalid then return empty
            if (existingRefreshToken.ExpiresAt < DateTime.UtcNow)
            {
                await MarkTokenAsInvalid(existingRefreshToken);
                return new TokenDto();
            }

            //replace old refresh token with new one and update expiry date
            var newRefreshToken = await CreateNewRefreshToken(existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);

            //revoke existing refresh token
            existingRefreshToken.IsValid = false;
            _dbContext.SaveChanges();

            //generate new access token
            var applicationUser = _dbContext.Users.FirstOrDefault(u=>u.Id==existingRefreshToken.UserId);
            if(applicationUser == null)
            {
                return new TokenDto();
            }
            var newAccessToken = await GetAccessToken(applicationUser,existingRefreshToken.JwtTokenId);

            return new TokenDto()
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
            };
        }
        public async Task RevokeRefreshToken(TokenDto tokenDto)
        {
            var existingRefreshToken = await _dbContext.RefreshTokens.FirstOrDefaultAsync(t=>t.Refresh_Token == tokenDto.RefreshToken);
            if (existingRefreshToken == null)
            {
                return;
            }
            // Compare data from existing refresh and access token provided and
            // if there is any missmatch then we should do nothing with refresh token
            var isTokenValid = GetAccessTokenData(tokenDto.AccessToken, existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);
            if (!isTokenValid)
            {
                return;
            }
            await MarkAllTokenInChainAsInvalid(existingRefreshToken.UserId, existingRefreshToken.JwtTokenId);
        }
        private async Task MarkAllTokenInChainAsInvalid(string userId, string tokenId)
        {
            await _dbContext.RefreshTokens.Where(u => u.UserId == userId
               && u.JwtTokenId == tokenId)
                   .ExecuteUpdateAsync(u => u.SetProperty(refreshToken => refreshToken.IsValid, false));

        }

        private Task MarkTokenAsInvalid(RefreshToken refreshToken)
        {
            refreshToken.IsValid = false;
            return _dbContext.SaveChangesAsync();
        }

    }
}
