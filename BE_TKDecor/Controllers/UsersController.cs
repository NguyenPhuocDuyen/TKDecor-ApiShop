using BusinessObject.Other;
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
using BusinessObject.DTO;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        private readonly ISendMailService _sendMailService;
        private IUserRepository _userRepository = new UserRepository();
        private IRoleRepository _roleRepository = new RoleRepository();
        private IRefreshTokenRepository _refreshTokenRepository = new RefreshTokenRepository();

        public UsersController(ISendMailService sendMailService, IOptions<JwtSettings> options)
        {
            _sendMailService = sendMailService;
            _jwtSettings = options.Value;
        }

        #region info user
        ////GET: api/Users/GetUsers
        //[Authorize(Roles = RoleContent.Admin)]
        //[HttpGet("GetUsers")]
        //public async Task<IActionResult> GetUsers()
        //{
        //    return Ok(new ApiResponse<IEnumerable<User>>
        //    {
        //        Success = true,
        //        Data = await _userRepository.GetAll()
        //    });
        //}

        // GET: api/Users/GetUserInfo
        [Authorize]
        [HttpGet("GetUserInfo")]
        public async Task<IActionResult> GetUserInfo()
        {
            User user = new();
            var currentUser = HttpContext.User;
            if (currentUser.HasClaim(c => c.Type == "UserId"))
            {
                var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
                // get user by user id
                user = await _userRepository.FindById(int.Parse(userId));
                return Ok(new ApiResponse<User>
                { Success = true, Data = user });
            }

            return BadRequest(new ApiResponse<User>
            { Success = false, Message = ErrorContent.Error });
        }

        //// POST: api/Users/UpdateUserInfo
        ////update user info
        //[Authorize]
        //[HttpPost("UpdateUserInfo")]
        //public async Task<ActionResult> UpdateInfoUser(User userInput)
        //{
        //    User user = new();
        //    //get user by token
        //    var currentUser = HttpContext.User;
        //    if (currentUser.HasClaim(c => c.Type == "UserId"))
        //    {
        //        var userId = currentUser.Claims.FirstOrDefault(c => c.Type == "UserId").Value;
        //        // get user by id
        //        user = await _db.User.GetFirstOrDefaultAsync(x => x.Id == int.Parse(userId));
        //    }

        //    //update orther info except password
        //    if (string.IsNullOrEmpty(userInput.Password))
        //    {
        //        //update user info not pass
        //        user.FullName = userInput.FullName;
        //        user.Phone = userInput.Phone;
        //        user.Address = userInput.Address;
        //    }
        //    else
        //    {
        //        //update password
        //        user.Password = HasPassword.HashPassword(userInput.Password);
        //    }
        //    user.UpdateAt = DateTime.Now;

        //    try
        //    {
        //        //update database
        //        _db.User.Update(user);
        //        await _db.SaveAsync();
        //        return NoContent();
        //    }
        //    catch
        //    {
        //        return BadRequest(new ErrorApp { Error = ErrorContent.Error });
        //    }
        //}
        #endregion

        // POST: api/Users/PostUser
        //user register accoount
        [HttpPost("Register")]
        public async Task<ActionResult> Register(UserRegisterDTO user)
        {
            user.Email = user.Email.ToLower().Trim();
            //get user in database by email
            User? u = await _userRepository.FindByEmail(user.Email);
            //check exists
            if (u != null)
                return BadRequest(new ApiResponse<object>
                { Success = false, Message = ErrorContent.EmailExists });

            try
            {
                // take customer role
                Role? role = await _roleRepository.FindByName(RoleContent.Customer);
                // get random code
                string code = GenerateRandomCode();
                // initial new user
                User newUser = new()
                {
                    RoleId = role.RoleId,
                    Email = user.Email,
                    Password = user.Password,
                    FullName = user.FullName,
                    AvatarUrl = user.AvatarUrl,
                    EmailConfirmed = false,
                    EmailConfirmationCode = code,
                    EmailConfirmationSentAt = DateTime.Now
                };
                //send mail to confirm account
                //set data to send
                MailContent mailContent = new()
                {
                    To = newUser.Email,
                    Subject = "Kích hoạt tài khoản TKDecor Shop",
                    Body = $"<h4>Bạn đã tạo tài khoản cho web TKDecor.</h4> <p>Đây là mã code của bạn: <strong>{code}</strong></p>"
                };
                // send mail
                await _sendMailService.SendMail(mailContent);
                // add user
                await _userRepository.Add(newUser);

                return NoContent();
            }
            catch
            {
                //lỗi do xung đột dữ liệu
                return BadRequest(new ApiResponse<object>
                { Success = false, Message = ErrorContent.Error });
            }
        }

        // POST: api/Users/Login
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginDTO user)
        {
            //get user by email
            user.Email = user.Email.ToLower().Trim();
            var u = await _userRepository.FindByEmail(user.Email);

            //check user null
            if (u == null)
                return NotFound(new ApiResponse<object>
                { Success = false, Message = ErrorContent.LoginFail });

            //check correct password
            bool isCorrectPassword = await _userRepository.CheckLogin(user.Email, user.Password);
            if (!isCorrectPassword)
                return BadRequest(new ApiResponse<object>
                { Success = false, Message = ErrorContent.LoginFail });

            //check confirm email
            if (u.EmailConfirmed is not true)
                return BadRequest(new ApiResponse<object>
                { Success = false, Message = ErrorContent.NotConfirmEmail });

            //convert token to string
            var token = await GenerateToken(u);

            return Ok(new ApiResponse<TokenModel> { Success = true, Data = token });
        }

        // POST: api/Users/ConfirmMail
        //confirm mail of user 
        [HttpPost("ConfirmMail")]
        public async Task<ActionResult> ConfirmMail(UserConfirmMailDTO userDto)
        {
            // get user by email confirm token 
            //var user = await _db.User.GetFirstOrDefaultAsync(x => x.EmailConfirmationToken == userInput.EmailConfirmationToken);
            var user = await _userRepository.FindByEmail(userDto.Email.ToLower().Trim());
            if (user == null)
                return NotFound(new ApiResponse<object>
                { Success = false, Message = ErrorContent.UserNotFound });

            if (user.EmailConfirmationCode != userDto.Code.Trim())
                return BadRequest(new ApiResponse<object>
                { Success = false, Message = "Wrong email confirmation code." });

            //set email confirm
            user.EmailConfirmed = true;
            try
            {
                //update database
                await _userRepository.Update(user);
                return NoContent();
            }
            catch
            {
                return BadRequest(new ApiResponse<object>
                { Success = false, Message = ErrorContent.Error });
            }
        }

        // POST: api/Users/ForgotPassword
        //ForgotPassword of user 
        [HttpPost("ForgotPassword")]
        public async Task<ActionResult> ForgotPassword([FromBody]string email)
        {
            // get user by email confirm token 
            var user = await _userRepository.FindByEmail(email);
            if (user == null)
                return NotFound(new ApiResponse<object>
                { Success = false, Message = ErrorContent.UserNotFound });

            try
            {
                // create code authen forgot password
                string code = GenerateRandomCode();
                //property authen mail
                user.ResetPasswordCode = code;
                user.ResetPasswordSentAt = DateTime.Now;
                user.ResetPasswordRequired = true;

                //send mail to confirm account
                //set data to send
                MailContent mailContent = new()
                {
                    To = user.Email,
                    Subject = "Đặt lại mật khẩu cho tài khoản TKDecor Shop",
                    Body = $"<h1>Nếu bạn không đặt lại mật khẩu cho tài khoản TKDecor Shop thì vui lòng bỏ qua email này</h1>" +
                       $"<h4>Nếu bạn đã đặt lại mật khẩu cho tài khoản TKDecor Shop thì hãy nhập mã code bên dưới.</h4>" +
                       $"<p>Đây là mã code của bạn: <strong>{code}</strong></p>"
                };

                // send mail
                await _sendMailService.SendMail(mailContent);
                //update user
                await _userRepository.Update(user);

                return NoContent();
            }
            catch
            {
                return BadRequest(new ApiResponse<object>
                { Success = false, Message = ErrorContent.Error });
            }
        }

        // POST: api/Users/ConfirmForgotPassword
        //ChangePassword of user 
        [HttpPost("ConfirmForgotPassword")]
        public async Task<ActionResult> ConfirmForgotPassword(UserConfirmForgotPasswordDTO userDto)
        {
            // get user by email
            var user = await _userRepository.FindByEmail(userDto.Email);

            if (user == null)
                return NotFound(new ApiResponse<object>
                { Success = false, Message = ErrorContent.UserNotFound });

            //check token expires: 5 minutes
            if (user.ResetPasswordSentAt <= DateTime.Now.AddMinutes(-5))
                return BadRequest(new ApiResponse<object>
                { Success = false, Message = ErrorContent.TokenOutDate });

            if (user.ResetPasswordRequired is not true)
                return BadRequest(new ApiResponse<object>
                { Success = false, Message = "Tài khoản không có yêu cầu đặt lại mật khẩu" });

            if (user.ResetPasswordCode != userDto.Code)
                return BadRequest(new ApiResponse<object>
                { Success = false, Message = "Sai mã code." });

            try
            {
                //if reset password, will back status do not reset password
                user.ResetPasswordRequired = false;
                //set new password
                user.Password = userDto.Password;

                //update to database and return info user
                await _userRepository.Update(user);
                return NoContent();
            }
            catch
            {
                return BadRequest(new ApiResponse<object>
                { Success = false, Message = ErrorContent.Error });
            }
        }

        [HttpPost("RenewToken")]
        public async Task<IActionResult> RenewToken(TokenModel model)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);
            var tokenValidateParam = new TokenValidationParameters
            {
                //tự cấp token
                ValidateIssuer = false,
                ValidateAudience = false,

                //ký vào token
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),

                ClockSkew = TimeSpan.Zero,

                ValidateLifetime = false //ko kiểm tra token hết hạn
            };
            try
            {
                //check 1: AccessToken valid format
                var tokenInVerification = jwtTokenHandler.ValidateToken(model.AccessToken, tokenValidateParam, out var validatedToken);

                //check 2: Check alg
                if (validatedToken is JwtSecurityToken jwtSecurityToken)
                {
                    var result = jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha512, StringComparison.InvariantCultureIgnoreCase);
                    if (!result)//false
                    {
                        return Ok(new ApiResponse<object>
                        { Success = false, Message = "Invalid token" });
                    }
                }

                //check 3: Check accessToken expire?
                var utcExpireDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                var expireDate = ConvertUnixTimeToDateTime(utcExpireDate);
                if (expireDate > DateTime.UtcNow)
                {
                    return Ok(new ApiResponse<object>
                    { Success = false, Message = "Access token has not yet expired" });
                }

                //check 4: Check refreshtoken exist in DB
                var storedToken = await _refreshTokenRepository.FindByToken(model.RefreshToken);
                if (storedToken == null)
                {
                    return Ok(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Refresh token does not exist"
                    });
                }

                //check 5: check refreshToken is used/revoked?
                if (storedToken.IsUsed)
                {
                    return Ok(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Refresh token has been used"
                    });
                }
                if (storedToken.IsRevoked)
                {
                    return Ok(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Refresh token has been revoked"
                    });
                }

                //check 6: AccessToken id == JwtId in RefreshToken
                var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                if (storedToken.JwtId != jti)
                {
                    return Ok(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Token doesn't match"
                    });
                }

                //Update token is used
                storedToken.IsRevoked = true;
                storedToken.IsUsed = true;
                await _refreshTokenRepository.Update(storedToken);

                //create new token
                var user = await _userRepository.FindById(storedToken.UserId);
                var token = await GenerateToken(user);

                return Ok(new ApiResponse<TokenModel>
                {
                    Success = true,
                    Message = "Renew token success",
                    Data = token
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Something went wrong"
                });
            }
        }

        private async Task<TokenModel> GenerateToken(User user)
        {
            //create handler
            var tokenHandler = new JwtSecurityTokenHandler();
            //encoding key in json
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
            //set description for token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("UserId", user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Role, user.Role.Name),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                }),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            //create token
            var token = tokenHandler.CreateToken(tokenDescriptor);
            //convert token to string
            var accessToken = tokenHandler.WriteToken(token);
            var refreshToken = GenerateReFreshToken();

            //Lưu database
            var refreshTokenEntity = new RefreshToken
            {
                Id = Guid.NewGuid(),
                JwtId = token.Id,
                UserId = user.UserId,
                Token = refreshToken,
                IsUsed = false,
                IsRevoked = false,
                IssuedAt = DateTime.UtcNow,
                ExpiredAt = DateTime.UtcNow.AddDays(7)
            };
            await _refreshTokenRepository.Add(refreshTokenEntity);

            return new TokenModel
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        private static string GenerateReFreshToken()
        {
            var random = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(random);

            return Convert.ToBase64String(random);
        }

        private static string GenerateRandomCode()
        {
            Random random = new();
            int code = random.Next(1000000, 9999999);
            return code.ToString();
        }
        
        private static DateTime ConvertUnixTimeToDateTime(long utcExpireDate)
        {
            var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeInterval.AddSeconds(utcExpireDate).ToUniversalTime();

            return dateTimeInterval;
        }
    }
}
