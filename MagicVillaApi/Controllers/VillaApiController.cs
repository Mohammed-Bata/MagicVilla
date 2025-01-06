using AutoMapper;
using MagicVillaApi.Data;
using MagicVillaApi.Models;
using MagicVillaApi.Models.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Net;

namespace MagicVillaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VillaApiController : ControllerBase
    {
        protected APIResponse _response;
        private readonly VillaDAO _villaDAO;
        private readonly IMapper _mapper;
        public VillaApiController(VillaDAO villaDAO,IMapper mapper)
        {
            this._response = new APIResponse();
            _villaDAO = villaDAO;
            _mapper = mapper;
        }
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ResponseCache(CacheProfileName = "default60")]
        public async Task<ActionResult<APIResponse>> GetVillas([FromQuery]string? search)
        {
            try
            {
                IEnumerable<Villa> Villas = await _villaDAO.GetAll();
                if (!string.IsNullOrEmpty(search))
                {
                    Villas = Villas.Where(v => v.Name.ToLower().Contains(search.ToLower()));
                }
                _response.Result = _mapper.Map<List<VillaDto>>(Villas);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Errors
                     = new List<string>() { ex.ToString() };
            }
            return _response;
        }
        [HttpGet("{id:int}",Name ="GetVilla")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<APIResponse>> GetVilla(int id)
        {
            try
            {
                if (id == 0)
                {
                    _response.StatusCode = HttpStatusCode.BadRequest;
                    _response.Errors.Add("Invalid Id");
                    return BadRequest(_response);
                }
                Villa villa = await _villaDAO.Get(v=>v.Id==id);
                if(villa == null)
                {
                    _response.StatusCode = HttpStatusCode.NotFound;
                    _response.Errors.Add("NotFound");
                    return NotFound(_response);
                }
                _response.Result = _mapper.Map<VillaDto>(villa);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Errors
                     = new List<string>() { ex.ToString() };
            }
            return _response;
        }
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<APIResponse>> CreateVilla([FromBody]VillaDto villaDto)
        {
            try
            {
                Villa villa = await _villaDAO.Get(v=>v.Name.ToLower() == villaDto.Name.ToLower());
                if(villa != null)
                {
                    ModelState.AddModelError("ErrorMessage", "Villa already Exists!");
                    return BadRequest(ModelState);
                }
                if (villaDto == null)
                {
                    return BadRequest(villaDto);
                }
                villa = _mapper.Map<Villa>(villaDto);
                villa.CreatedAt = DateTime.Now;
                await _villaDAO.CreateVilla(villa);

                _response.Result = _mapper.Map<VillaDto>(villa);
                _response.StatusCode=HttpStatusCode.Created;

                return CreatedAtRoute("GetVilla", new { id = villa.Id }, _response);
            }
            catch(Exception  ex)
            {
                _response.IsSuccess = false;
                _response.Errors
                     = new List<string>() { ex.ToString() };
            }
            return _response;
        }
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<APIResponse>> DeleteVilla(int id)
        {
            try
            {
                if(id == 0)
                {
                    return BadRequest();
                }
                Villa villa = await _villaDAO.Get(v => v.Id==id);
                if(villa == null)
                {
                    return NotFound();
                }
                await _villaDAO.DeleteVilla(villa);
                _response.StatusCode =HttpStatusCode.NoContent;
                return Ok(_response);
            }
            catch(Exception ex)
            {
                _response.IsSuccess = false;
                _response.Errors
                     = new List<string>() { ex.ToString() };
            }
            return _response;
        }
        [HttpPut("{id:int}")]
        public async Task<ActionResult<APIResponse>> Update(int id,VillaDto villadto)
        {
            try
            {
                if (villadto == null || id == 0)
                {
                    return BadRequest();
                }
                Villa villa = await _villaDAO.Get(v=>v.Name.ToLower() ==villadto.Name.ToLower(),false);
                if (villa != null&&villa.Id != id)
                {
                    ModelState.AddModelError("ErrorMessage", "Villa already Exists!");
                    return BadRequest(ModelState);
                }
                villa = await _villaDAO.Get(v => v.Id == id, false);
                DateTime create = villa.CreatedAt;
                if (villa == null)
                {
                    return NotFound();
                }
                villa = _mapper.Map<Villa>(villadto);
                villa.CreatedAt = create;
                villa.UpdatedAt = DateTime.Now;
                await _villaDAO.UpdateVilla(villa);
                _response.StatusCode = HttpStatusCode.OK;
                return Ok(_response);
            }
            catch (Exception ex)
            {
                _response.IsSuccess = false;
                _response.Errors
                     = new List<string>() { ex.ToString() };
            }
            return _response;
        }
        [HttpPatch("{id:int}")]
        public async Task<IActionResult> UpdatePartialVilla(int id, JsonPatchDocument<VillaDto> patchDTO)
        {
            if (patchDTO == null || id == 0)
            {
                return BadRequest();
            }
            Villa villa = await _villaDAO.Get(v=>v.Id == id, false);
            if(villa == null)
            {
                return NotFound();
            }
            VillaDto villaDto = _mapper.Map<VillaDto>(villa);
            patchDTO.ApplyTo(villaDto, ModelState);
            Villa model = _mapper.Map<Villa>(villaDto);

            await _villaDAO.UpdateVilla(model);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            return NoContent();
        }
    }
}
