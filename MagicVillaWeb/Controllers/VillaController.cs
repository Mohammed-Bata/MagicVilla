using AutoMapper;
using MagicVillaWeb.Models;
using MagicVillaWeb.Models.Dtos;
using MagicVillaWeb.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using Utility;

namespace MagicVillaWeb.Controllers
{
    public class VillaController : Controller
    {
        private readonly VillaService _villaService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public VillaController(VillaService villaService, IMapper mapper,IWebHostEnvironment webHostEnvironment)
        {
            _villaService = villaService;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }
        public async Task<IActionResult> Index()
        {
            List<VillaDto> list = new List<VillaDto>();
            var response = await _villaService.GetAllAsync<APIResponse>();

            if (response != null && response.IsSuccess)
            {
                list = JsonConvert.DeserializeObject<List<VillaDto>>(Convert.ToString(response.Result));
            }
            return View(list);
        }
        public async Task<IActionResult> Create()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(VillaDto dto)
        {
          
            if (ModelState.IsValid)
            {
                var response = await _villaService.CreateAsync<APIResponse>(dto);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Villa created successfully";
                    return RedirectToAction(nameof(Index));
                }
            }
            TempData["error"] = "Error encountered.";
            return View(dto);
        }
        public async Task<IActionResult> Update(int id)
        {
            var response = await _villaService.GetAsync<APIResponse>(id);
            if (response != null && response.IsSuccess)
            {
                VillaDto dto = JsonConvert.DeserializeObject<VillaDto>(Convert.ToString(response.Result));
                return View(dto);
            }
            return NotFound();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(VillaDto dto)
        {
            if (ModelState.IsValid)
            {
                var response = await _villaService.UpdateAsync<APIResponse>(dto);
                if (response != null && response.IsSuccess)
                {
                    TempData["success"] = "Villa Updated successfully";
                    return RedirectToAction(nameof(Index));
                }
            }
            TempData["error"] = "Error encountered.";
            return View(dto);
        }
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _villaService.GetAsync<APIResponse>(id);
            if(response != null && response.IsSuccess)
            {
                VillaDto dto = JsonConvert.DeserializeObject<VillaDto>(Convert.ToString(response.Result));
                return View(dto);
            }
            return NotFound();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(VillaDto dto)
        {
            var response = await _villaService.DeleteAsync<APIResponse>(dto.Id);
            if (response != null && response.IsSuccess)
            {
                TempData["success"] = "Villa deleted successfully";
                return RedirectToAction(nameof(Index));
            }
            TempData["error"] = "Error encountered.";
            return View(dto);
        }
    }
}
