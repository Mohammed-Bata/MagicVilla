using MagicVillaWeb.Models;
using MagicVillaWeb.Models.Dtos;
using static Utility.SD;

namespace MagicVillaWeb.Services
{
    public class AuthService:BaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private string villaUrl;
        public AuthService(IHttpClientFactory clientFactory, IConfiguration configuration):base(clientFactory)
        {
            _httpClientFactory = clientFactory;
            villaUrl = configuration.GetValue<string>("ServiceUrls:VillaApi");
        }
        public Task<T> LoginAsync<T>(LoginRequestDto dto)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = ApiType.Post,
                Data = dto,
                Url = villaUrl + "/api/UserApi/Login",
            });
        }
        public Task<T> RegisterAsync<T>(RegisterationRequestDto dto)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = ApiType.Post,
                Data = dto,
                Url = villaUrl + "/api/UserApi/Register",
            });
        }
    }
}
