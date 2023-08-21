using AutoMapper;
using BE_TKDecor.Core.Config.JWT;
using BE_TKDecor.Core.Dtos.Notification;
using BE_TKDecor.Core.Dtos.User;
using BE_TKDecor.Core.Mail;
using BE_TKDecor.Core.Response;
using BE_TKDecor.Hubs;
using BE_TKDecor.Service.IService;
using BusinessObject;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Utility;

namespace BE_TKDecor.Service
{
    public class AuthenService : IAuthenService
    {
        private readonly TkdecorContext _context;
        private readonly ISendMailService _sendMailService;
        private readonly IHubContext<NotificationHub> _hub;
        private readonly IMapper _mapper;
        private readonly JwtSettings _jwtSettings;
        private ApiResponse _response;

        public AuthenService(TkdecorContext context,
            IOptions<JwtSettings> options,
            ISendMailService sendMailService,
            IHubContext<NotificationHub> hub,
            IMapper mapper)
        {
            _context = context;
            _sendMailService = sendMailService;
            _hub = hub;
            _mapper = mapper;
            _jwtSettings = options.Value;
            _response = new ApiResponse();
        }

        public async Task<ApiResponse> Login(UserLoginDto dto)
        {
            //get user by email
            dto.Email = dto.Email.ToLower().Trim();
            //var u = await _user.FindByEmail(dto.Email);

            var user = _context.Users.FirstOrDefault(x => x.Email == dto.Email);

            //check user null
            if (user is null)
            {
                _response.Message = ErrorContent.AccountIncorrect;
                return _response;
            }

            if (user.IsDelete)
            {
                _response.Message = "Tài khoản đã bị chặn!";
                return _response;
            }

            //check correct password
            if (!Password.VerifyPassword(dto.Password, user.Password))
            {
                _response.Message = ErrorContent.AccountIncorrect;
                return _response;
            }

            //check confirm email
            if (!user.EmailConfirmed)
            {
                _response.Message = "Email chưa được xác nhận!";
                return _response;
            }

            //convert token to string
            var token = await GenerateToken(user);

            var data = new
            {
                user.Role,
                user.Email,
                user.FullName,
                user.AvatarUrl,
                token.AccessToken,
                token.RefreshToken,
            };
            _response.Success = true;
            _response.Data = data;
            return _response;
        }

        public async Task<ApiResponse> Register(UserRegisterDto dto)
        {
            dto.Email = dto.Email.ToLower().Trim();
            //get user in database by email
            User? user = await _context.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);

            //check exists
            bool isAdd = true;
            if (user is not null)
            {
                isAdd = false;
                if (user.EmailConfirmed == true)
                {
                    if (user.IsDelete == true)
                    {
                        _response.Message = "Tài khoản đã bị chặn";
                    }
                    else
                    {
                        _response.Message = "Email đã tồn tại!";
                    }
                    return _response;
                }
            }
            else
            {
                // initial new user
                // take customer role
                user = new User
                {
                    Email = dto.Email,
                    Role = SD.RoleCustomer,
                    Gender = SD.GenderOther
                };
            }

            // get random code
            string code = RandomCode.GenerateRandomCode();
            user.Password = Password.HashPassword(dto.Password);

            user.FullName = dto.FullName;

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
                    await _context.Users.AddAsync(user);
                }
                else
                {
                    _context.Users.Update(user);
                }
                await _context.SaveChangesAsync();
                _response.Success = true;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        public async Task<ApiResponse> ConfirmMail(UserConfirmMailDto dto)
        {
            dto.Email = dto.Email.ToLower().Trim();

            // get user by email confirm token 
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == dto.Email);

            if (user is null)
            {
                _response.Message = ErrorContent.UserNotFound;
                return _response;
            }

            if (user.EmailConfirmationCode != dto.Code.Trim())
            {
                _response.Message = "Sai mã xác nhận!";
                return _response;
            }

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
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                if (codeOutOfDate)
                {
                    _response.Message = "Mã xác nhận đã hết hạn sử dụng. Vui lòng kiểm tra lại mã xác nhận mới trong email của bạn!";
                    return _response;
                }

                Notification newNotification = new()
                {
                    UserId = user.UserId,
                    Message = "Chào mừng bạn tới web TKDecor của chúng tôi"
                };
                _context.Notifications.Add(newNotification);
                await _context.SaveChangesAsync();

                _response.Success = true;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        public async Task<ApiResponse> ResendConfirmationEmail(UserEmailDto dto)
        {
            // get user by email 
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == dto.Email.ToLower().Trim());
            if (user is null)
            {
                _response.Message = ErrorContent.UserNotFound;
                return _response;
            }

            if (user.EmailConfirmed)
            {
                _response.Message = "Email đã được xác nhận!";
                return _response;
            }

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
                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                _response.Success = true;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        public async Task<ApiResponse> ForgotPassword(UserEmailDto dto)
        {
            // get user by email 
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == dto.Email.ToLower().Trim());
            if (user is null)
            {
                _response.Message = ErrorContent.UserNotFound;
                return _response;
            }

            if (!user.EmailConfirmed)
            {
                _response.Message = "Email chưa được xác nhận!";
                return _response;
            }

            if (user.IsDelete)
            {
                _response.Message = "Tài khoản đã bị chặn";
                return _response;
            }

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
                //update user
                _context.Users.Update(user);

                Notification newNotification = new()
                {
                    UserId = user.UserId,
                    Message = "Bạn đã yêu cầu quên mật khẩu"
                };
                _context.Notifications.Add(newNotification);
                await _hub.Clients.User(user.UserId.ToString()).SendAsync(SD.NewNotification, _mapper.Map<NotificationGetDto>(newNotification));

                await _context.SaveChangesAsync();
                _response.Success = true;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        public async Task<ApiResponse> ConfirmForgotPassword(UserConfirmForgotPasswordDto dto)
        {
            // get user by email confirm token 
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == dto.Email.ToLower().Trim());
            if (user is null)
            {
                _response.Message = ErrorContent.UserNotFound;
                return _response;
            }

            if (user.IsDelete)
            {
                _response.Message = "Tài khoản đã bị chặn";
                return _response;
            }

            if (!user.ResetPasswordRequired)
            {
                _response.Message = "Tài khoản không yêu cầu đặt lại mật khẩu!";
                return _response;
            }

            if (user.ResetPasswordCode != dto.Code)
            {
                _response.Message = "Sai mã xác nhận!";
                return _response;
            }

            //check token expires: 5 minutes
            if (user.ResetPasswordSentAt <= DateTime.Now.AddMinutes(-1))
            {
                _response.Message = "Mã xác nhận hết hạn!";
                return _response;
            }

            //if reset password, will back status do not reset password
            user.ResetPasswordRequired = false;
            //set new password
            user.Password = Password.HashPassword(dto.Password);
            user.UpdatedAt = DateTime.Now;
            try
            {
                //update to database and return info user
                _context.Users.Update(user);

                Notification newNotification = new()
                {
                    UserId = user.UserId,
                    Message = "Xác nhận mật khẩu mới thành công"
                };
                _context.Notifications.Add(newNotification);
                await _hub.Clients.User(user.UserId.ToString()).SendAsync(SD.NewNotification, _mapper.Map<NotificationGetDto>(newNotification));

                await _context.SaveChangesAsync();
                _response.Success = true;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        public async Task<ApiResponse> RenewToken(TokenModel model)
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
                    {
                        _response.Message = "Mã xác thực không hợp lệ";
                        return _response;
                    }
                }

                //check 3: Check accessToken expire?
                var utcExpireDate = long.Parse(tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

                var expireDate = ConvertUnixTimeToDateTime(utcExpireDate);
                if (expireDate > DateTime.Now)
                {
                    _response.Message = "Mã xác thực truy cập chưa hết hạn!";
                    return _response;
                }

                //check 4: Check refreshtoken exist in DB
                var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == model.RefreshToken);
                if (storedToken is null)
                {
                    _response.Message = "Mã xác thực làm mới không tồn tại!";
                    return _response;
                }

                //check 5: check refreshToken is used/revoked?
                if (storedToken.IsUsed)
                {
                    _response.Message = "Mã xác thực làm mới đã được sử dụng!";
                    return _response;
                }

                if (storedToken.IsRevoked)
                {
                    _response.Message = "Mã xác thực làm mới đã bị thu hồi!";
                    return _response;
                }

                //check 6: AccessToken id == JwtId in RefreshToken
                var jti = tokenInVerification.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;
                if (storedToken.JwtId != jti)
                {
                    _response.Message = "Mã xác thực không khớp!";
                    return _response;
                }

                //Update token is used
                storedToken.IsRevoked = true;
                storedToken.IsUsed = true;
                _context.RefreshTokens.Update(storedToken);
                await _context.SaveChangesAsync();

                //create new token
                var user = await _context.Users.FirstOrDefaultAsync(x => x.UserId == storedToken.UserId);
                var token = await GenerateToken(user);

                _response.Success = true;
                _response.Message = "Gia hạn mã xác thực thành công";
                _response.Data = token;
            }
            catch { _response.Message = ErrorContent.Data; }
            return _response;
        }

        private async Task<TokenModel> GenerateToken(User user)
        {
            //create handler
            var tokenHandler = new JwtSecurityTokenHandler();
            //encoding key in json
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

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
            _context.RefreshTokens.Add(refreshTokenEntity);
            await _context.SaveChangesAsync();

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
            var dateTimeInterval = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Local);
            dateTimeInterval.AddSeconds(utcExpireDate).ToUniversalTime();
            return dateTimeInterval;
        }
    }
}
