using BusinessObject;
using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Utility.Mail;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using BE_TKDecor.Core.Dtos.User;
using BE_TKDecor.Core.Response;
using DataAccess.StatusContent;
using BE_TKDecor.Core.Config.JWT;
using AutoMapper;
using Utility;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _userRepository;

        public UsersController(ISendMailService sendMailService, 
            IOptions<JwtSettings> options,
            IMapper mapper,
            IUserRepository userRepository,
            IRoleRepository roleRepository,
            IRefreshTokenRepository refreshTokenRepository)
        {
            _mapper = mapper;
            _userRepository = userRepository;
        }

        // GET: api/Users/GetUserInfo
        [Authorize]
        [HttpGet("GetUserInfo")]
        public async Task<IActionResult> GetUserInfo()
        {
            User? user;
            var currentUser = HttpContext.User;
            if (currentUser.HasClaim(c => c.Type == "UserId"))
            {
                var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
                // get user by user id
                user = await _userRepository.FindById(int.Parse(userId));

                var result = _mapper.Map<UserGetDto>(user);

                return Ok(new ApiResponse
                { Success = true, Data = result });
            }

            return BadRequest(new ApiResponse { Message = ErrorContent.Error });
        }
    }
}
