using AutoMapper;
using MagicVillaWeb.Models;
using MagicVillaWeb.Models.Dtos;
using MagicVillaWeb.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Utility;

namespace MagicVillaWeb.Controllers
{
    public class AuthController : Controller
    {
        private readonly AuthService _authService;
        private readonly IMapper _mapper;
        public AuthController(AuthService service,IMapper mapper)
        {
            _authService = service;
            _mapper = mapper;
        }
        [HttpGet]
        public IActionResult Login()
        {
            LoginRequestDto obj = new();
            return View(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginRequestDto dto)
        {
            APIResponse response = await _authService.LoginAsync<APIResponse>(dto);
            if(response != null && response.IsSuccess)
            {
                LoginResponseDto model = JsonConvert.DeserializeObject<LoginResponseDto>(Convert.ToString(response.Result));
                var handler = new JwtSecurityTokenHandler();
                var jwt = handler.ReadJwtToken(model.token);

                var identity = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
                identity.AddClaim(new Claim(ClaimTypes.Name, jwt.Claims.FirstOrDefault(u => u.Type == "unique_name").Value));
                identity.AddClaim(new Claim(ClaimTypes.Role, jwt.Claims.FirstOrDefault(u => u.Type == "role").Value));

                var principal = new ClaimsPrincipal(identity);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                HttpContext.Session.SetString(SD.SessionToken,model.token);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("CustomError", response.Errors.FirstOrDefault());
                return View(dto);
            }
        }
        [HttpGet]
        public IActionResult Register()
        {
            RegisterationRequestDto obj = new();
            return View(obj);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterationRequestDto dto)
        {
            var response = await _authService.RegisterAsync<APIResponse>(dto);
            if(response!=null && response.IsSuccess)
            {
                return RedirectToAction("Login");
            }
            return View();
        }
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            HttpContext.Session.SetString(SD.SessionToken, "");
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

    }
}
