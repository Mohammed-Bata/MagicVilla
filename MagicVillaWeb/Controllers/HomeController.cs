using MagicVillaWeb.Models;
using MagicVillaWeb.Models.Dtos;
using MagicVillaWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using Utility;

namespace MagicVillaWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly VillaService _villaService;

        public HomeController(VillaService villaService)
        {
            _villaService = villaService;
        }
        public async Task<IActionResult> Index()
        {
            List<VillaDto> list = new List<VillaDto>();
            var response = await _villaService.GetAllAsync<APIResponse>(HttpContext.Session.GetString(SD.SessionToken));

            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<VillaDto>>(Convert.ToString(response.Result));
            }
            return View(list);
        }

    }
}
