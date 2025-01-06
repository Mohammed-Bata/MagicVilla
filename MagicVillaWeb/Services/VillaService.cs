using MagicVillaWeb.Models.Dtos;
using System.Configuration;
using static Utility.SD;
using System.Security.Policy;
using Utility;
using MagicVillaWeb.Models;
using Humanizer;
using NuGet.Common;

namespace MagicVillaWeb.Services
{
    public class VillaService : BaseService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private string villaUrl;
        public VillaService(IHttpClientFactory clientFactory,IConfiguration configuration):base(clientFactory)
        {
            _httpClientFactory = clientFactory;
            villaUrl = configuration.GetValue<string>("ServiceUrls:VillaApi");
        }
        public Task<T> CreateAsync<T>(VillaDto dto,string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = ApiType.Post,
                Data = dto,
                Url = villaUrl + "/api/VillaApi",
                Token = token
            });
        }

        public Task<T> DeleteAsync<T>(int id,string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = ApiType.Delete,
                Url = villaUrl + "/api/VillaApi/"+id,
                Token = token,
            });
        }

        public Task<T> GetAllAsync<T>(string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.Get,
                Url = villaUrl + "/api/VillaApi",
                Token = token
            });
        }

        public Task<T> GetAsync<T>(int id,string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.Get,
                Url = villaUrl + "/api/VillaApi/"+id,
                Token = token,
            });
        }

        public Task<T> UpdateAsync<T>(VillaDto dto,string token)
        {
            return SendAsync<T>(new APIRequest()
            {
                ApiType = ApiType.Put,
                Data = dto,
                Url = villaUrl + "/api/VillaApi/"+dto.Id,
                Token = token,
            });
        }

    }
}
