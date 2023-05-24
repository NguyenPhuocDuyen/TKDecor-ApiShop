using BusinessObject.Other;
using BusinessObject;
using DataAccess.Repository;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Utility.Mail;
using Utility;
using BusinessObject.DTO;
using System.Security.Cryptography.Xml;
using Microsoft.AspNetCore.Http.HttpResults;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ISendMailService _sendMailService;
        private IUserRepository userRepository = new UserRepository();
        private IRoleRepository roleRepository = new RoleRepository();

        public UsersController(IConfiguration configuration, ISendMailService sendMailService)
        {
            _configuration = configuration;
            _sendMailService = sendMailService;
        }

        ////GET: api/Users/GetUsers
        //[Authorize(Roles = RoleContent.Admin)]
        //[HttpGet("GetUsers")]
        //public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        //{
        //    return Ok(await _db.User.GetAllAsync());
        //}

        ////GET: api/Users/TotalUser
        //[HttpGet("TotalUser")]
        //public async Task<ActionResult<int>> TotalUser()
        //{
        //    var list = await _db.User.GetAllAsync();
        //    return Ok(list.Count());
        //}

        //// GET: api/Users/GetUserInfo
        //[Authorize]
        //[HttpGet("GetUserInfo")]
        //public async Task<ActionResult<User>> GetUserInfo()
        //{
        //    User user = new();
        //    var currentUser = HttpContext.User;
        //    if (currentUser.HasClaim(c => c.Type == ClaimTypes.Name))
        //    {
        //        var userId = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
        //        // get user by user id
        //        user = await _db.User.GetFirstOrDefaultAsync(x => x.Id == int.Parse(userId));
        //    }

        //    return user;
        //}

        // POST: api/Users/PostUser
        //user register accoount
        [HttpPost("Register")]
        public async Task<ActionResult> Register(RegisterUserDTO user)
        {
            user.Email = user.Email.ToLower().Trim();
            user.Password = user.Password.ToLower().Trim();

            //get user in database by email
            User? u = await userRepository.FindByEmail(user.Email);

            //check exists
            if (u != null && u.EmailConfirmed is false)
                return BadRequest(new ErrorApp { Error = ErrorContent.EmailExists });

            try
            {
                // take customer role
                Role? role = await roleRepository.FindByName(RoleContent.Customer);

                string code = Token.GenerateRandomCode();
                User newUser = new()
                {
                    RoleId = role.RoleId,
                    Email = user.Email,
                    PasswordHash = Password.HashPassword(user.Password),
                    FullName = user.FullName,
                    AvatarUrl = user.AvatarUrl,
                    IsSubscriber = false,
                    EmailConfirmed = false,
                    EmailConfirmationCode = code,
                    EmailConfirmationSentAt = DateTime.Now,
                    ResetPasswordRequired = false,
                    ResetPasswordCode = null,
                    ResetPasswordSentAt = null,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    IsDelete = false
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

                var resultUser = await userRepository.Add(newUser);

                return Ok();
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.ToString());
                //lỗi do xung đột dữ liệu
                return Conflict(new ErrorApp { Error = ex.ToString()});
            }
        }

        // POST: api/Users/Login
        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<ActionResult> Login(LoginUserDTO user)
        {
            //get user by email
            user.Email = user.Email.ToLower().Trim();
            var u = await userRepository.FindByEmail(user.Email);

            //check user null
            if (u == null) return NotFound(new ErrorApp { Error = ErrorContent.LoginFail });

            //check confirm email
            if (u.EmailConfirmed is false) return BadRequest(new ErrorApp { Error = ErrorContent.NotConfirmEmail });

            //check correct password
            bool isCorrectPassword = Password.VerifyPassword(user.Password, u.PasswordHash);

            if (!isCorrectPassword)
                return BadRequest(new ErrorApp { Error = ErrorContent.LoginFail });

            //create handler
            var tokenHandler = new JwtSecurityTokenHandler();
            //encoding key in json
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]);
            //set description for token
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, u.UserId.ToString()),
                    new Claim(ClaimTypes.Role, u.Role.Name.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            //create token
            var token = tokenHandler.CreateToken(tokenDescriptor);
            //convert token to string
            var tokenString = tokenHandler.WriteToken(token);
            u.PasswordHash= "";

            return Ok(new { User = u, Access_Token = tokenString });
        }

        // POST: api/Users/ConfirmMail
        //confirm mail of user 
        [HttpPost("ConfirmMail")]
        public async Task<ActionResult> ConfirmMail(User userInput)
        {
            // get user by email confirm token 
            //var user = await _db.User.GetFirstOrDefaultAsync(x => x.EmailConfirmationToken == userInput.EmailConfirmationToken);
            var user = await userRepository.FindByEmail(userInput.Email);

            if (user == null)
                return NotFound(new ErrorApp { Error = ErrorContent.UserNotFound });

            if (user.EmailConfirmationCode != userInput.EmailConfirmationCode)
                return BadRequest(new ErrorApp { Error = "Wrong email confirmation code." });

            //set email confirm
            user.EmailConfirmed = true;

            try
            {
                //update database
                await userRepository.Update(user);
                return NoContent();
            }
            catch
            {
                return BadRequest(new ErrorApp { Error = ErrorContent.Error });
            }
        }

        //// POST: api/Users/UpdateInfoUser
        ////update user info
        //[Authorize]
        //[HttpPost("UpdateInfoUser")]
        //public async Task<ActionResult> UpdateInfoUser(User userInput)
        //{
        //    User user = new();
        //    //get user by token
        //    var currentUser = HttpContext.User;
        //    if (currentUser.HasClaim(c => c.Type == ClaimTypes.Name))
        //    {
        //        var userId = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
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

        //// POST: api/Users/ForgotPassword
        ////ForgotPassword of user 
        //[HttpPost("ForgotPassword")]
        //public async Task<ActionResult> ForgotPassword(User userInput)
        //{
        //    // get user by email confirm token 
        //    var user = await _db.User.GetFirstOrDefaultAsync(
        //        filter: x => x.Email.ToLower().Trim() == userInput.Email.ToLower().Trim());

        //    if (user == null) return NotFound(new ErrorApp { Error = ErrorContent.UserNotFound });

        //    try
        //    {
        //        // create code authen forgot password
        //        byte[] randomBytes = new byte[32];
        //        using (RNGCryptoServiceProvider rng = new())
        //        {
        //            rng.GetBytes(randomBytes);
        //        }
        //        string token = BitConverter.ToString(randomBytes).Replace("-", "").ToLower();

        //        //property authen mail
        //        user.ResetPasswordToken = token;
        //        user.ResetPasswordSentAt = DateTime.Now;
        //        user.IsPasswordResetRequired = true;

        //        //send mail to confirm account
        //        //set data to send
        //        MailContent mailContent = new()
        //        {
        //            To = user.Email,
        //            Subject = "Đặt lại mật khẩu cho tài khoản TKDecor Shop",
        //            Body = $"<h1>Nếu bạn không đặt lại mật khẩu cho tài khoản TKDecor Shop thì vui lòng bỏ qua email này</h1>" +
        //               $"<h4>Nếu bạn đã đặt lại mật khẩu cho tài khoản TKDecor Shop thì click vào link dưới</h4>" +
        //               $"<p>Vui lòng click vào <a href=\"https://localhost:44310/Account/ResetPassword?tokenPassword={token}&email={user.Email}\">đây</a> để kích hoạt tài khoản của bạn.</p>"
        //        };

        //        //send mail
        //        HttpResponseMessage response = GobalVariables.WebAPIClient.PostAsJsonAsync($"Mails/PostMail", mailContent).Result;
        //        //check status
        //        if (response.IsSuccessStatusCode)
        //        {
        //            //update to database
        //            _db.User.Update(user);
        //            await _db.SaveAsync();

        //            return NoContent();
        //        }
        //        else
        //        {
        //            return BadRequest(new ErrorApp { Error = ErrorContent.SendEmail });
        //        }
        //    }
        //    catch { }

        //    return BadRequest(new ErrorApp { Error = ErrorContent.Error });
        //}

        //// POST: api/Users/ChangePassword/tokenPassword
        ////ChangePassword of user 
        //[HttpPost("ChangePassword/{tokenPassword}")]
        //public async Task<ActionResult> ChangePassword(string tokenPassword, User userInput)
        //{
        //    // get user by email confirm token 
        //    var user = await _db.User.GetFirstOrDefaultAsync(
        //        filter: x => x.Email.ToLower().Trim() == userInput.Email.ToLower().Trim()
        //        && x.ResetPasswordToken == tokenPassword
        //        && x.IsPasswordResetRequired == true);

        //    if (user == null) return NotFound(new ErrorApp { Error = ErrorContent.UserNotFound });

        //    //check token expires 
        //    if (user.ResetPasswordSentAt <= DateTime.Now.AddHours(-1))
        //        return BadRequest(new ErrorApp { Error = ErrorContent.TokenOutDate });

        //    try
        //    {
        //        //if reset password, will back status do not reset password
        //        user.IsPasswordResetRequired = false;
        //        //set new password
        //        user.Password = HasPassword.HashPassword(userInput.Password);

        //        //update to database
        //        _db.User.Update(user);
        //        await _db.SaveAsync();

        //        return NoContent();
        //    }
        //    catch { }

        //    return BadRequest(new ErrorApp { Error = ErrorContent.Error });
        //}
    }
}
