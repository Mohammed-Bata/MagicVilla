using AutoMapper;
using MagicVillaApi.Models;
using MagicVillaApi.Models.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
            RoleManager<IdentityRole> roleManager, IConfiguration configuration,IMapper mapper)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _RoleManager = roleManager;
            _mapper = mapper;
            SecretKey = configuration.GetValue<string>("ApiSettings:Secret");
        }
        public bool IsUnique(string username)
        {
            var user = _dbContext.Users.FirstOrDefault(u=>u.UserName == username);
            if (user == null)
            {
                return true;
            }
                return false;
        }
        public async Task<LoginResponseDto> Login(LoginRequestDto dto)
        {
            var User =  _dbContext.Users.FirstOrDefault(u=>u.UserName.ToLower() == dto.UserName.ToLower());
            bool isvalid = await _userManager.CheckPasswordAsync(User,dto.Password);

            if (User == null||!isvalid)
            {
                return new LoginResponseDto()
                {
                    token = "",
                    user = null,
                };
            }

            var roles = await _userManager.GetRolesAsync(User);
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(SecretKey);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, User.UserName),
                    new Claim(ClaimTypes.Role, roles.FirstOrDefault()),
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new(new SymmetricSecurityKey(key),SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            LoginResponseDto loginResponseDto = new LoginResponseDto()
            {
                token = tokenHandler.WriteToken(token),
                user = _mapper.Map<UserDto>(User),
            };
            return loginResponseDto;
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
                if(!_RoleManager.RoleExistsAsync("Admin").GetAwaiter().GetResult())
                {
                    await _RoleManager.CreateAsync(new IdentityRole("Admin"));
                    await _RoleManager.CreateAsync(new IdentityRole("Client"));
                }
                await _userManager.AddToRoleAsync(user,dto.Role);
                var userToReturn = _dbContext.Users
                    .FirstOrDefault(u => u.UserName == dto.UserName);
                return _mapper.Map<UserDto>(userToReturn);
            }
            return new UserDto();
        }
        
    }
}
