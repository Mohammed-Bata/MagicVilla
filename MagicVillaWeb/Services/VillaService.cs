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
    public class VillaService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private string villaUrl;
        private readonly BaseService _baseService;
        public VillaService(IHttpClientFactory clientFactory,IConfiguration configuration, BaseService baseService)
        {
            _httpClientFactory = clientFactory;
            villaUrl = configuration.GetValue<string>("ServiceUrls:VillaApi");
            _baseService = baseService;
        }
        public async Task<T> CreateAsync<T>(VillaDto dto)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = ApiType.Post,
                Data = dto,
                Url = villaUrl + "/api/VillaApi",
                ContentType = ContentType.MultipartFormData
            });
        }

        public async Task<T> DeleteAsync<T>(int id)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = ApiType.Delete,
                Url = villaUrl + "/api/VillaApi/"+id,
            });
        }

        public async Task<T> GetAllAsync<T>()
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.Get,
                Url = villaUrl + "/api/VillaApi",
            });
        }

        public async Task<T> GetAsync<T>(int id)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = SD.ApiType.Get,
                Url = villaUrl + "/api/VillaApi/"+id,
            });
        }

        public async Task<T> UpdateAsync<T>(VillaDto dto)
        {
            return await _baseService.SendAsync<T>(new APIRequest()
            {
                ApiType = ApiType.Put,
                Data = dto,
                Url = villaUrl + "/api/VillaApi/"+dto.Id,
                ContentType = ContentType.MultipartFormData
            });
        }

    }
}
