using AutoMapper.Internal;
using MagicVillaWeb.Models;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using Utility;

namespace MagicVillaWeb.Services
{
    public class BaseService
    {
        public APIResponse Response { get; set; }
        public IHttpClientFactory httpClient { get; set; }

        public BaseService(IHttpClientFactory httpClient)
        {
            this.Response = new();
            this.httpClient = httpClient;
        }
        public async Task<T> SendAsync<T>(APIRequest request)
        {
            try
            {
                var client = httpClient.CreateClient("MagicApi");

                HttpRequestMessage message = new HttpRequestMessage();
                message.Headers.Add("Accept", "application/json");
                message.RequestUri = new Uri(request.Url);
                if(request.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(request.Data),
                        Encoding.UTF8, "application/json");
                }

                switch (request.ApiType)
                {
                    case SD.ApiType.Post:
                        message.Method = HttpMethod.Post;
                        break;
                    case SD.ApiType.Put:
                        message.Method = HttpMethod.Put;
                        break;
                    case SD.ApiType.Delete:
                        message.Method = HttpMethod.Delete;
                        break;
                    default:
                        message.Method = HttpMethod.Get;
                        break;
                }

                HttpResponseMessage response = null;
                if (!string.IsNullOrEmpty(request.Token))
                {
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", request.Token);
                }

                response = await client.SendAsync(message);
                var apiContent = await response.Content.ReadAsStringAsync();
                try
                {
                    APIResponse ApiResponse = JsonConvert.DeserializeObject<APIResponse>(apiContent);
                    if (ApiResponse != null && (ApiResponse.StatusCode == System.Net.HttpStatusCode.BadRequest
                       || ApiResponse.StatusCode == System.Net.HttpStatusCode.NotFound))
                    {
                        ApiResponse.StatusCode = System.Net.HttpStatusCode.BadRequest;
                        ApiResponse.IsSuccess = false;
                        var res = JsonConvert.SerializeObject(ApiResponse);
                        var returnObj = JsonConvert.DeserializeObject<T>(res);
                        return returnObj;
                    }
                }
                catch (Exception ex)
                {
                    var exceptionResponse = JsonConvert.DeserializeObject<T>(apiContent);
                    return exceptionResponse;
                }
                var APIResponse = JsonConvert.DeserializeObject<T>(apiContent);
                return APIResponse;
            }
            catch (Exception ex)
            {
                var dto = new APIResponse
                {
                    Errors = new List<string> { Convert.ToString(ex.Message) },
                    IsSuccess = false
                };
                var res = JsonConvert.SerializeObject(dto);
                var APIResponse = JsonConvert.DeserializeObject<T>(res);
                return APIResponse;
            }
        }
    }
}
