using AutoMapper;
using MagicVillaApi.Data;
using MagicVillaApi.Models;
using MagicVillaApi.Models.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVillaApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserApiController : ControllerBase
    {
        protected APIResponse _response;
        private readonly ApplicationUserDAO _userDAO;

        public UserApiController(ApplicationUserDAO userDAO)
        {
            _userDAO = userDAO;
            _response = new APIResponse();
        }
        [HttpPost("Login")]
        public async Task<ActionResult<APIResponse>> Login(LoginRequestDto dto)
        {
            var loginResponse = await _userDAO.Login(dto);
            if(loginResponse.user == null || string.IsNullOrEmpty(loginResponse.token))
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.Errors.Add("Username or password is incorrect");
                return BadRequest(_response);
            }
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = loginResponse;
            return Ok(_response);
        }
        [HttpPost("Register")]
        public async Task<ActionResult<APIResponse>> Register(RegisterationRequestDto dto)
        {
            bool isunique = _userDAO.IsUnique(dto.UserName);
            if (!isunique)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.Errors.Add("Username Already Exist");
                return BadRequest(_response);
            }
            var user = await _userDAO.Register(dto);
            if (user == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.Errors.Add("Error");
                return BadRequest(_response);
            }
            _response.StatusCode=HttpStatusCode.OK;
            _response.IsSuccess=true;
            return Ok(_response);
        }
    }
}
