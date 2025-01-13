using Humanizer;
using MagicVillaWeb.Models;
using MagicVillaWeb.Models.Dtos;
using static Utility.SD;

namespace MagicVillaWeb.Services
{
    public class AuthService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private string villaUrl;
        private readonly BaseService _baseService;
        public AuthService(IHttpClientFactory clientFactory, IConfiguration configuration,BaseService baseService)
        {
            _httpClientFactory = clientFactory;
            villaUrl = configuration.GetValue<string>("ServiceUrls:VillaApi");
            _baseService = baseService;
        }
        public async Task<T> LoginAsync<T>(LoginRequestDto dto)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = ApiType.Post,
                Data = dto,
                Url = villaUrl + "/api/UserApi/Login",
            },withBearer:false);
        }
        public async Task<T> RegisterAsync<T>(RegisterationRequestDto dto)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = ApiType.Post,
                Data = dto,
                Url = villaUrl + "/api/UserApi/Register",
            },withBearer:false);
        }
        public async Task<T> LogoutAsync<T>(TokenDto obj)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = ApiType.Post,
                Data = obj,
                Url = villaUrl + "/api/UserApi/Revoke",
            });
        }
    }
}
