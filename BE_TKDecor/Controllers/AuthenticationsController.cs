using BE_TKDecor.Core.Config.JWT;
using BE_TKDecor.Core.Dtos.User;
using BE_TKDecor.Core.Response;
using BusinessObject;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Utility.Mail;
using Utility;
using AutoMapper;
using DataAccess.Repository.IRepository;
using Microsoft.Extensions.Options;
using BE_TKDecor.Hubs;
using Microsoft.AspNetCore.SignalR;
using BE_TKDecor.Core.Dtos.Notification;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationsController : ControllerBase
    {
        private readonly JwtSettings _jwtSettings;
        private readonly ISendMailService _sendMailService;
        private readonly IUserRepository _user;
        private readonly IRefreshTokenRepository _refreshToken;
        private readonly INotificationRepository _notification;
        private readonly IHubContext<NotificationHub> _hub;
        private readonly IMapper _mapper;

        public AuthenticationsController(ISendMailService sendMailService,
            IOptions<JwtSettings> options,
            IUserRepository user,
            IRefreshTokenRepository refreshToken,
            INotificationRepository notification,
            IHubContext<NotificationHub> hub,
            IMapper mapper)
        {
            _sendMailService = sendMailService;
            _jwtSettings = options.Value;
            _user = user;
            _refreshToken = refreshToken;
            _notification = notification;
            _hub = hub;
            _mapper = mapper;
        }

        // POST: api/Authentications/Register
        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserRegisterDto userDto)
        {
            userDto.Email = userDto.Email.ToLower().Trim();
            //get user in database by email
            User? user = await _user.FindByEmail(userDto.Email);
            bool isAdd = true;
            //check exists
            if (user != null)
            {
                if (user.EmailConfirmed == true)
                {
                    return BadRequest(new ApiResponse
                    { Message = "Email đã tồn tại!" });
                }
                isAdd = false;
            }

            if (isAdd)
            {
                // initial new user
                // take customer role
                user = new User
                {
                    Email = userDto.Email,
                    AvatarUrl = "",
                    Role = SD.RoleCustomer,
                    EmailConfirmed = false
                };
            }

            // get random code
            string code = RandomCode.GenerateRandomCode();
            user.Password = Password.HashPassword(userDto.Password);

            user.FullName = userDto.FullName;
            user.BirthDay = userDto.BirthDay;
            user.Gender = userDto.Gender;

            user.EmailConfirmationCode = code;
            user.EmailConfirmationSentAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;

            //send mail to confirm account
            //set data to send
            MailContent mailContent = new()
            {
                To = user.Email,
                Subject = "Kích hoạt tài khoản TKDecor Shop",
                Body = $"<h4>Bạn đã tạo tài khoản cho web TKDecor.</h4> <p>Đây là mã xác nhận của bạn: <strong>{code}</strong></p>"
            };
            // send mail
            await _sendMailService.SendMail(mailContent);

            try
            {
                if (isAdd)
                {
                    // add user
                    await _user.Add(user);
                }
                else
                {
                    await _user.Update(user);
                }
                return Ok(new ApiResponse { Success = true });
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        // POST: api/Authentications/ConfirmMail
        //confirm mail of user 
        [HttpPost("ConfirmMail")]
        public async Task<ActionResult> ConfirmMail(UserConfirmMailDto userDto)
        {
            // get user by email confirm token 
            var user = await _user.FindByEmail(userDto.Email.ToLower().Trim());
            if (user == null)
                return NotFound(new ApiResponse
                { Message = ErrorContent.UserNotFound });

            if (user.EmailConfirmationCode != userDto.Code.Trim())
                return BadRequest(new ApiResponse
                { Message = "Sai mã xác nhận!" });

            bool codeOutOfDate = false;
            if (user.EmailConfirmationSentAt <= DateTime.Now.AddMinutes(-5))
            {
                codeOutOfDate = true;
                string code = RandomCode.GenerateRandomCode();
                user.EmailConfirmationCode = code;
                user.EmailConfirmationSentAt = DateTime.Now;
                //send mail to confirm account
                //set data to send
                MailContent mailContent = new()
                {
                    To = user.Email,
                    Subject = "Kích hoạt tài khoản TKDecor Shop",
                    Body = $"<h4>Bạn đã tạo tài khoản cho web TKDecor.</h4> <p>Đây là mã xác nhận của bạn: <strong>{code}</strong></p>"
                };
                // send mail
                await _sendMailService.SendMail(mailContent);
            }
            else
            {
                //set email confirm
                user.EmailConfirmed = true;
            }
            user.UpdatedAt = DateTime.Now;
            try
            {
                await _user.Update(user);
                if (codeOutOfDate)
                {
                    return BadRequest(new ApiResponse { Message = "Mã xác nhận đã hết hạn sử dụng. Vui lòng kiểm tra lại mã xác nhận mới trong email của bạn!" });
                }
                Notification newNotification = new()
                {
                    UserId = user.UserId,
                    Message = "Chào mừng bạn tới web TKDecor của chúng tôi"
                };
                await _notification.Add(newNotification);
                
                return Ok(new ApiResponse { Success = true });
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        // POST: api/Authentications/ResendConfirmationCode
        [HttpPost("ResendConfirmationCode")]
        public async Task<IActionResult> ResendConfirmationEmail(UserEmailDto userDto)
        {
            // get user by email confirm token 
            var user = await _user.FindByEmail(userDto.Email.ToLower().Trim());
            if (user == null)
                return NotFound(new ApiResponse
                { Message = ErrorContent.UserNotFound });

            if (user.EmailConfirmed is true)
                return BadRequest(new ApiResponse
                { Message = "Email đã được xác nhận!" });

            //check code expires to create new code: 5 minutes
            if (user.EmailConfirmationSentAt <= DateTime.Now.AddMinutes(-5))
                user.EmailConfirmationCode = RandomCode.GenerateRandomCode();

            user.EmailConfirmationSentAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;

            //send mail to confirm account
            //set data to send
            MailContent mailContent = new()
            {
                To = user.Email,
                Subject = "Kích hoạt tài khoản TKDecor Shop",
                Body = $"<h4>Bạn đã tạo tài khoản cho web TKDecor.</h4> " +
                $"<p>Đây là mã xác nhận của bạn: <strong>{user.EmailConfirmationCode}</strong></p>"
            };
            // send mail
            await _sendMailService.SendMail(mailContent);

            try
            {
                // add user
                await _user.Update(user);
                return Ok(new ApiResponse { Success = true });
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        // POST: api/Authentications/Login
        [HttpPost("Login")]
        public async Task<IActionResult> Login(UserLoginDto userDto)
        {
            //get user by email
            userDto.Email = userDto.Email.ToLower().Trim();
            var u = await _user.FindByEmail(userDto.Email);

            //check user null
            if (u == null)
                return NotFound(new ApiResponse
                { Message = ErrorContent.AccountIncorrect });

            if (u.IsDelete)
                return BadRequest(new ApiResponse { Message = "Tài khoản bị chặn!" });

            //check correct password
            bool isCorrectPassword = Password.VerifyPassword(userDto.Password, u.Password);
            if (!isCorrectPassword)
                return BadRequest(new ApiResponse
                { Message = ErrorContent.AccountIncorrect });

            //check confirm email
            if (!u.EmailConfirmed)
                return BadRequest(new ApiResponse
                { Message = "Email chưa được xác nhận!" });

            //convert token to string
            var token = await GenerateToken(u);
            //string roleString = Enum.GetName(typeof(Role), u.Role);

            var data = new
            {
                u.Role,
                u.Email,
                u.FullName,
                u.AvatarUrl,
                token.AccessToken,
                token.RefreshToken,
            };

            return Ok(new ApiResponse { Success = true, Data = data });
        }

        // POST: api/Authentications/ForgotPassword
        //ForgotPassword of user 
        [HttpPost("ForgotPassword")]
        public async Task<ActionResult> ForgotPassword(UserEmailDto userDto)
        {
            // get user by email confirm token 
            var user = await _user.FindByEmail(userDto.Email);
            if (user == null)
                return NotFound(new ApiResponse
                { Message = ErrorContent.UserNotFound });

            // create code authen forgot password
            string code = RandomCode.GenerateRandomCode();
            //property authen mail
            user.ResetPasswordCode = code;
            user.ResetPasswordSentAt = DateTime.Now;
            user.ResetPasswordRequired = true;
            user.UpdatedAt = DateTime.Now;

            //send mail to confirm account
            //set data to send
            MailContent mailContent = new()
            {
                To = user.Email,
                Subject = "Đặt lại mật khẩu tài khoản TKDecor Shop",
                Body = $"<h1>Nếu bạn không đặt lại mật khẩu cho tài khoản TKDecor Shop, vui lòng bỏ qua email này</h1>" +
                   $"<h4>Nếu bạn đã đặt lại mật khẩu cho tài khoản Shop TKDecor của mình, vui lòng nhập mã xác nhận bên dưới.</h4>" +
                   $"<p>Đây là mã xác nhận của bạn: <strong>{code}</strong></p>"
            };
            // send mail
            await _sendMailService.SendMail(mailContent);

            try
            {
                //update user/
                await _user.Update(user);

                Notification newNotification = new()
                {
                    UserId = user.UserId,
                    Message = "Bạn đã yêu cầu quên mật khẩu"
                };
                await _notification.Add(newNotification);
                await _hub.Clients.User(user.UserId.ToString()).SendAsync(SD.NewNotification, _mapper.Map<NotificationGetDto>(newNotification));
                return Ok(new ApiResponse { Success = true });
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        // POST: api/Authentications/ConfirmForgotPassword
        //ChangePassword of user 
        [HttpPost("ConfirmForgotPassword")]
        public async Task<ActionResult> ConfirmForgotPassword(UserConfirmForgotPasswordDto userDto)
        {
            // get user by email
            var user = await _user.FindByEmail(userDto.Email);

            if (user == null)
                return NotFound(new ApiResponse
                { Message = ErrorContent.UserNotFound });

            if (!user.ResetPasswordRequired)
                return BadRequest(new ApiResponse
                { Message = "Tài khoản không yêu cầu đặt lại mật khẩu!" });

            //check token expires: 5 minutes
            if (user.ResetPasswordSentAt <= DateTime.Now.AddMinutes(-5))
                return BadRequest(new ApiResponse
                { Message = "Mã xác nhận hết hạn!" });

            if (user.ResetPasswordCode != userDto.Code)
                return BadRequest(new ApiResponse
                { Message = "Sai mã xác nhận!" });

            //if reset password, will back status do not reset password
            user.ResetPasswordRequired = false;
            //set new password
            user.Password = Password.HashPassword(userDto.Password);
            user.UpdatedAt = DateTime.Now;
            try
            {
                //update to database and return info user
                await _user.Update(user);

                Notification newNotification = new()
                {
                    UserId = user.UserId,
                    Message = "Xác nhận mật khẩu mới thành công"
                };
                await _notification.Add(newNotification);
                await _hub.Clients.User(user.UserId.ToString()).SendAsync(SD.NewNotification, _mapper.Map<NotificationGetDto>(newNotification));

                return Ok(new ApiResponse { Success = true });
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        // POST: api/Authentications/RenewToken
        [HttpPost("RenewToken")]
        public async Task<IActionResult> RenewToken(TokenModel model)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var secretKeyBytes = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);
            var tokenValidateParam = new TokenValidationParameters
            {
                //self-grant tokens
                ValidateIssuer = false,
                ValidateAudience = false,

                //sign the token
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(secretKeyBytes),

                ClockSkew = TimeSpan.Zero,

                //do not check expired token
                ValidateLifetime = false
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
                        return Ok(new ApiResponse
                        { Message = "Mã xác thực không hợp lệ" });
                }

                //check 3: Check accessToken expire?
                var utcExpireDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                var expireDate = ConvertUnixTimeToDateTime(utcExpireDate);
                if (expireDate > DateTime.Now)
                    return Ok(new ApiResponse
                    { Message = "Mã xác thực truy cập chưa hết hạn!" });

                //check 4: Check refreshtoken exist in DB
                var storedToken = await _refreshToken.FindByToken(model.RefreshToken);
                if (storedToken == null)
                    return Ok(new ApiResponse
                    { Message = "Mã xác thực làm mới không tồn tại!" });

                //check 5: check refreshToken is used/revoked?
                if (storedToken.IsUsed)
                    return Ok(new ApiResponse
                    { Message = "Mã xác thực làm mới đã được sử dụng!" });

                if (storedToken.IsRevoked)
                    return Ok(new ApiResponse
                    { Message = "Mã xác thực làm mới đã bị thu hồi!" });

                //check 6: AccessToken id == JwtId in RefreshToken
                var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                if (storedToken.JwtId != jti)
                    return Ok(new ApiResponse
                    { Message = "Mã xác thực không khớp!" });

                //Update token is used
                storedToken.IsRevoked = true;
                storedToken.IsUsed = true;
                await _refreshToken.Update(storedToken);

                //create new token
                var user = await _user.FindById(storedToken.UserId);
                var token = await GenerateToken(user);

                return Ok(new ApiResponse
                {
                    Success = true,
                    Message = "Gia hạn mã xác thực thành công",
                    Data = token
                });
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        private async Task<TokenModel> GenerateToken(User user)
        {
            //create handler
            var tokenHandler = new JwtSecurityTokenHandler();
            //encoding key in json
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);
            //set description for token
            //string roleString = Enum.GetName(typeof(Role), user.Role);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim("UserId", user.UserId.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.FullName),
                    new Claim(ClaimTypes.Role, user.Role),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                }),
                Expires = DateTime.Now.AddMinutes(15),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };
            //create token
            var token = tokenHandler.CreateToken(tokenDescriptor);
            //convert token to string
            var accessToken = tokenHandler.WriteToken(token);
            var refreshToken = GenerateReFreshToken();

            //save database
            var refreshTokenEntity = new RefreshToken
            {
                RefreshTokenId = Guid.NewGuid(),
                JwtId = token.Id,
                UserId = user.UserId,
                Token = refreshToken,
                IsUsed = false,
                IsRevoked = false,
                IssuedAt = DateTime.Now,
                ExpiredAt = DateTime.Now.AddMonths(1)
            };
            await _refreshToken.Add(refreshTokenEntity);

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

        private static DateTime ConvertUnixTimeToDateTime(long utcExpireDate)
        {
            var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTimeInterval.AddSeconds(utcExpireDate).ToUniversalTime();

            return dateTimeInterval;
        }
    }
}
