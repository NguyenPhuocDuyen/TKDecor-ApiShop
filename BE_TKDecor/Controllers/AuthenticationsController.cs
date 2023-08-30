using BE_TKDecor.Core.Dtos.User;
using BE_TKDecor.Core.Response;
using Microsoft.AspNetCore.Mvc;
using BE_TKDecor.Service.IService;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationsController : ControllerBase
    {
        private readonly IAuthenService _authen;

        public AuthenticationsController(IAuthenService authen)
        {
            _authen = authen;
        }

        // POST: api/Authentications/Register
        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserRegisterDto dto)
        {
            var res = await _authen.Register(dto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // POST: api/Authentications/ConfirmMail
        //confirm mail of user 
        [HttpPost("ConfirmMail")]
        public async Task<ActionResult> ConfirmMail(UserConfirmMailDto dto)
        {
            var res = await _authen.ConfirmMail(dto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // POST: api/Authentications/ResendConfirmationCode
        [HttpPost("ResendConfirmationCode")]
        public async Task<IActionResult> ResendConfirmationEmail(UserEmailDto dto)
        {
            var res = await _authen.ResendConfirmationEmail(dto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // POST: api/Authentications/Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginDto dto)
        {
            var res = await _authen.Login(dto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // POST: api/Authentications/ForgotPassword
        //ForgotPassword of user 
        [HttpPost("ForgotPassword")]
        public async Task<ActionResult> ForgotPassword(UserEmailDto dto)
        {
            var res = await _authen.ForgotPassword(dto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res); 
        }

        // POST: api/Authentications/ConfirmForgotPassword
        //ChangePassword of user 
        [HttpPost("ConfirmForgotPassword")]
        public async Task<ActionResult> ConfirmForgotPassword(UserConfirmForgotPasswordDto dto)
        {
            var res = await _authen.ConfirmForgotPassword(dto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // POST: api/Authentications/RenewToken
        [HttpPost("RenewToken")]
        public async Task<IActionResult> RenewToken(TokenModel dto)
        {
            var res = await _authen.RenewToken(dto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }
    }
}
