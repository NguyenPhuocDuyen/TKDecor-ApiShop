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
using Utility;
using BusinessObject.DTO;

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

        //GET: api/Users/GetUsers
        [Authorize(Roles = RoleContent.Admin)]
        [HttpGet("GetUsers")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return Ok(await userRepository.GetAll());
        }

        ////GET: api/Users/TotalUser
        //[HttpGet("TotalUser")]
        //public async Task<ActionResult<int>> TotalUser()
        //{
        //    var list = await _db.User.GetAllAsync();
        //    return Ok(list.Count());
        //}

        // GET: api/Users/GetUserInfo
        [Authorize]
        [HttpGet("GetUserInfo")]
        public async Task<ActionResult<User>> GetUserInfo()
        {
            User user = new();
            var currentUser = HttpContext.User;
            if (currentUser.HasClaim(c => c.Type == ClaimTypes.Name))
            {
                var userId = currentUser.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name).Value;
                // get user by user id
                user = await userRepository.FindById(int.Parse(userId));
                user.PasswordHash = "";
                return user;
            }
            return Conflict(new ErrorApp { Error = ErrorContent.Error });
        }

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
                    //IsSubscriber = false,
                    //EmailConfirmed = false,
                    //EmailConfirmationCode = code,
                    //EmailConfirmationSentAt = DateTime.Now,
                    //ResetPasswordRequired = false,
                    //ResetPasswordCode = null,
                    //ResetPasswordSentAt = null,
                    //CreatedAt = DateTime.Now,
                    //UpdatedAt = DateTime.Now,
                    //IsDelete = false
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

                return Ok(resultUser);
            }
            catch 
            {
                //await Console.Out.WriteLineAsync(ex.ToString());
                //lỗi do xung đột dữ liệu
                return Conflict(new ErrorApp { Error = ErrorContent.Error});
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
            u.PasswordHash = "";

            return Ok(new { User = u, Access_Token = tokenString });
        }

        // POST: api/Users/ConfirmMail
        //confirm mail of user 
        [HttpPost("ConfirmMail")]
        public async Task<ActionResult> ConfirmMail(string email, string code)
        {
            // get user by email confirm token 
            //var user = await _db.User.GetFirstOrDefaultAsync(x => x.EmailConfirmationToken == userInput.EmailConfirmationToken);
            var user = await userRepository.FindByEmail(email.ToLower().Trim());

            if (user == null)
                return NotFound(new ErrorApp { Error = ErrorContent.UserNotFound });

            if (user.EmailConfirmationCode != code.Trim())
                return BadRequest(new ErrorApp { Error = "Wrong email confirmation code." });

            //set email confirm
            user.EmailConfirmed = true;

            try
            {
                //update database
                var userInfo = await userRepository.Update(user);
                return Ok(userInfo);
            }
            catch
            {
                return BadRequest(new ErrorApp { Error = ErrorContent.Error });
            }
        }

        // POST: api/Users/ForgotPassword
        //ForgotPassword of user 
        [HttpPost("ForgotPassword")]
        public async Task<ActionResult> ForgotPassword(string email)
        {
            // get user by email confirm token 
            var user = await userRepository.FindByEmail(email);

            if (user == null) return NotFound(new ErrorApp { Error = ErrorContent.UserNotFound });

            try
            {
                // create code authen forgot password
                string code = Token.GenerateRandomCode();
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
                var userInfo = await userRepository.Update(user);

                return Ok(userInfo);
            }
            catch
            {
                return BadRequest(new ErrorApp { Error = ErrorContent.Error });
            }
        }

        // POST: api/Users/ConfirmForgotPassword
        //ChangePassword of user 
        [HttpPost("ConfirmForgotPassword")]
        public async Task<ActionResult> ConfirmForgotPassword(LoginUserDTO userReset, string code)
        {
            // get user by email
            var user = await userRepository.FindByEmail(userReset.Email);

            if (user == null) return NotFound(new ErrorApp { Error = ErrorContent.UserNotFound });

            //check token expires: 5 minutes
            if (user.ResetPasswordSentAt <= DateTime.Now.AddMinutes(-5))
                return BadRequest(new ErrorApp { Error = ErrorContent.TokenOutDate });

            if (user.ResetPasswordRequired is not true)
                return BadRequest(new ErrorApp { Error = "Tài khoản không có yêu cầu đặt lại mật khẩu" });

            if (user.ResetPasswordCode != code) 
                return BadRequest(new ErrorApp { Error = "Sai mã code." });

            try
            {
                //if reset password, will back status do not reset password
                user.ResetPasswordRequired = false;
                //set new password
                user.PasswordHash = Password.HashPassword(userReset.Password.Trim());

                //update to database and return info user
                var userInfo = await userRepository.Update(user);
                return Ok(userInfo);
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
    }
}
