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
        public async Task<IActionResult> Register(UserRegisterDto userDto)
        {
            var res = await _authen.Register(userDto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // POST: api/Authentications/ConfirmMail
        //confirm mail of user 
        [HttpPost("ConfirmMail")]
        public async Task<ActionResult> ConfirmMail(UserConfirmMailDto userDto)
        {
            var res = await _authen.ConfirmMail(userDto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // POST: api/Authentications/ResendConfirmationCode
        [HttpPost("ResendConfirmationCode")]
        public async Task<IActionResult> ResendConfirmationEmail(UserEmailDto userDto)
        {
            var res = await _authen.ResendConfirmationEmail(userDto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // POST: api/Authentications/Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginDto userDto)
        {
            var res = await _authen.Login(userDto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // POST: api/Authentications/ForgotPassword
        //ForgotPassword of user 
        [HttpPost("ForgotPassword")]
        public async Task<ActionResult> ForgotPassword(UserEmailDto userDto)
        {
            var res = await _authen.ForgotPassword(userDto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res); 
        }

        // POST: api/Authentications/ConfirmForgotPassword
        //ChangePassword of user 
        [HttpPost("ConfirmForgotPassword")]
        public async Task<ActionResult> ConfirmForgotPassword(UserConfirmForgotPasswordDto userDto)
        {
            var res = await _authen.ConfirmForgotPassword(userDto);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }

        // POST: api/Authentications/RenewToken
        [HttpPost("RenewToken")]
        public async Task<IActionResult> RenewToken(TokenModel model)
        {
            var res = await _authen.RenewToken(model);
            if (res.Success)
            {
                return Ok(res);
            }
            return BadRequest(res);
        }
    }
}
