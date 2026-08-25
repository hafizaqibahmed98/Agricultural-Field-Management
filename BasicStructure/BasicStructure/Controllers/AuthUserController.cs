using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Services.Models;
using Services.Services;

namespace BasicStructure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthUserController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;
        public AuthUserController(IAuthService authService, IEmailService emailService)
        {
            _authService = authService;
            _emailService = emailService;
        }
        
        [HttpPost("Register")]
        public async Task<bool> RegisterUser(RegisterUserDTO user, int role)
        {
            return await _authService.RegisterUser(user, role, HttpContext);

        }

        [HttpPost("Login")]
        public async Task<IActionResult> LoginUser(Models.LoginUser user)
        {
            var result =  await _authService.LoginUser(user);
            if(result == true)
            {
                var tokenString = _authService.GenerateTokenString(user);
                return Ok(tokenString);
            }
            return BadRequest(result);

        }

        //[HttpGet("EmailTest")]
        //public IActionResult TestEmailService()
        //{
        //    var message = new Message(
        //        new string[] { "aqib02541@gmail.com" },
        //        "Testing Email Service",
        //        "Hi, this is test mail"
        //        );
        //    _emailService.SendEmail(message);
        //    return Ok("Email Sent Successfully");
        //}

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var result = await _authService.ConfirmEmail(token, email);
            if(result==true)
                return Ok("Email Verified Successfully");
            return BadRequest("This User Doesnot exist!");
        }

        [HttpPost("ForgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var result = await _authService.ForgotPassword(email, HttpContext);
            if (result == true)
                return Ok("Reset Password Email Send Successfully");
            return BadRequest("Failure Reset Password Email!");
        }

        [HttpPost("ResetPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPassword resetPassword)
        {
            var result = await _authService.ResetPassword(resetPassword);
            if (result == true)
                return Ok("Password Changed Successfully");
            return BadRequest("Failure!");
        }
    }
}
