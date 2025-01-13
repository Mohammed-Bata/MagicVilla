using Microsoft.AspNetCore.Mvc;
using static Utility.SD;

namespace MagicVillaWeb.Models
{
    public class APIRequest
    {
        public ApiType ApiType { get; set; } = ApiType.Get;
        public string Url {  get; set; }
        public object Data {  get; set; }
        public string Token {  get; set; }

        public ContentType ContentType { get; set; } = ContentType.Json;
    }
}
