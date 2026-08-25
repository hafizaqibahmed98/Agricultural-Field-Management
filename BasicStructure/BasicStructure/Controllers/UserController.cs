using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BasicFlowApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class UserController : ControllerBase
    {
        private readonly InterfaceUserService _userService;
        private readonly IAuthService _authService;


        public UserController(InterfaceUserService userService, IAuthService authService)
        {
            this._userService = userService;
            this._authService = authService;
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<GetUserDTO>>>> GetAllUsers()
        {
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            // Decode the token
            User user = _authService.DecodeToken(token);
            bool permissionAllowed = await _authService.CheckPermission(user, 1);
            if (permissionAllowed)
            {
                return Ok(await _userService.GetAllUsers());
            }
            return BadRequest("Forbidden");
        }

        [HttpGet("id")]
        public async Task<ActionResult<ServiceResponse<GetUserDTO>>> GetUserById(int id)
        {
            return Ok(await _userService.GetById(id));
        }

        [HttpPost]
        public async Task<ActionResult<ServiceResponse<List<GetUserDTO>>>> AddUser(AddUserDTO user)
        {
            return Ok(await _userService.AddUser(user));
        }

        [HttpPut("id")]
        public async Task<ActionResult<ServiceResponse<List<GetUserDTO>>>> UpdateUserById(int id, AddUserDTO user)
        {
            return Ok(await _userService.UpdateUser(id, user));
        }

        [HttpDelete("id")]
        public async Task<ActionResult<ServiceResponse<List<GetUserDTO>>>> DeleteUserById(int id)
        {
            return Ok(await _userService.DeleteUser(id));
        }

    }
}
