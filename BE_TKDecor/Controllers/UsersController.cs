using BusinessObject;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BE_TKDecor.Core.Dtos.User;
using BE_TKDecor.Core.Response;
using AutoMapper;
using Utility;
using Utility.Mail;
using System.Net;
using System.Text;
using System.Net.Http.Headers;
using Utility.SD;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IUserRepository _user;
        private readonly ISendMailService _sendMailService;

        public UsersController(IMapper mapper,
            IUserRepository user,
            ISendMailService sendMailService)
        {
            _mapper = mapper;
            _user = user;
            _sendMailService = sendMailService;
        }

        // GET: api/Users/GetUserInfo
        [HttpGet("GetUserInfo")]
        public async Task<IActionResult> GetUserInfo()
        {
            // get user by user id
            var user = await GetUser();
            var result = _mapper.Map<UserGetDto>(user);

            return Ok(new ApiResponse
            { Success = true, Data = result });
        }

        // GET: api/Users/UpdateUserInfo
        [HttpPost("UpdateUserInfo")]
        public async Task<IActionResult> UpdateUserInfo(UserUpdateDto userDto)
        {
            var user = await GetUser();
            if (user == null)
                return BadRequest(new ApiResponse
                { Message = ErrorContent.UserNotFound });

            if (!Enum.TryParse(userDto.Gender, out Gender gender))
                return BadRequest(new ApiResponse { Message = ErrorContent.GenderNotFound });

            user.FullName = userDto.FullName;
            user.AvatarUrl = userDto.AvatarUrl;
            user.BirthDay = userDto.BirthDay;
            user.Gender = gender;
            user.UpdatedAt = DateTime.Now;
            try
            {
                await _user.Update(user);
                return Ok(new ApiResponse { Success = true });
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        // GET: api/Users/RequestChangePassword
        [HttpPost("RequestChangePassword")]
        public async Task<IActionResult> RequestChangePassword()
        {
            var user = await GetUser();
            if (user == null)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            // get random code
            string code = RandomCode.GenerateRandomCode();
            user.ResetPasswordRequired = true;
            user.ResetPasswordCode = code;
            user.ResetPasswordSentAt = DateTime.Now;
            user.UpdatedAt = DateTime.Now;

            try
            {
                // update user
                await _user.Update(user);

                //send mail to changepassword
                //set data to send
                MailContent mailContent = new()
                {
                    To = user.Email,
                    Subject = "Change password at TKDecor Shop",
                    Body = $"<h4>You have requested to change the password for the TKDecor web site. " +
                    $"If you do not have a request to change your password, ignore this email!</h4>" +
                    $"<p>Here is your code: <strong>{code}</strong></p>"
                };
                // send mail
                await _sendMailService.SendMail(mailContent);

                return Ok(new ApiResponse { Success = true });
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        // GET: api/Users/ChangePassword
        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(UserChangePasswordDto userDto)
        {
            var user = await GetUser();
            if (user == null)
                return NotFound(new ApiResponse { Message = ErrorContent.UserNotFound });

            if (!user.ResetPasswordRequired is not true)
                return BadRequest(new ApiResponse { Message = "User is not required to change password!" });

            if (user.ResetPasswordCode != userDto.Code)
                return BadRequest(new ApiResponse { Message = "Wrong code!" });

            //check correct password
            bool isCorrectPassword = Password.VerifyPassword(userDto.Password, user.Password);
            if (!isCorrectPassword)
                return BadRequest(new ApiResponse
                { Message = "Wrong password!" });

            bool codeOutOfDate = false;
            if (user.ResetPasswordSentAt > DateTime.Now.AddMinutes(-5))
            {
                codeOutOfDate = true;
            }

            // random new code if code time expired
            if (codeOutOfDate)
            {
                // get random code
                string code = RandomCode.GenerateRandomCode();
                user.ResetPasswordCode = code;
            }
            else
            {
                user.Password = Password.HashPassword(userDto.NewPassword);
                user.ResetPasswordRequired = false;
            }
            user.UpdatedAt = DateTime.Now;
            try
            {
                // update success before send new code for user
                await _user.Update(user);

                if (codeOutOfDate)
                {
                    //send mail to changepassword
                    //set data to send
                    MailContent mailContent = new()
                    {
                        To = user.Email,
                        Subject = "Change password at TKDecor Shop",
                        Body = $"<h4>You have requested to change the password for the TKDecor web site. " +
                        $"If you do not have a request to change your password, ignore this email!</h4>" +
                        $"<p>Here is your code: <strong>{user.ResetPasswordCode}</strong></p>"
                    };
                    // send mail
                    await _sendMailService.SendMail(mailContent);
                    return BadRequest(new ApiResponse { Message = "Password change time expired!. Pls check mail again to see new code!" });
                }
                return Ok(new ApiResponse { Success = true });
            }
            catch { return BadRequest(new ApiResponse { Message = ErrorContent.Data }); }
        }

        [HttpGet]
        public async Task<HttpResponseMessage> ExportToCsv()
        {
            var user = await GetUser();

            // Tạo đối tượng StringBuilder để xây dựng chuỗi CSV
            var csv = new StringBuilder();

            // Ghi tiêu đề của các cột vào chuỗi CSV
            csv.AppendLine("FullName,Email,Column3");

            // Ghi dữ liệu từng dòng vào chuỗi CSV
            //foreach (var item in data)
            //{
            csv.AppendLine($"{user.FullName},{user.Email}");
            //}

            // Tạo đối tượng HttpResponseMessage
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(csv.ToString())
            };
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("text/csv");
            response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
            {
                FileName = "data.csv"
            };

            return response;
        }

        private async Task<User?> GetUser()
        {
            var currentUser = HttpContext.User;
            if (currentUser.HasClaim(c => c.Type == "UserId"))
            {
                var userId = currentUser?.Claims?.FirstOrDefault(c => c.Type == "UserId")?.Value;
                // get user by user id
                if (userId != null)
                    return await _user.FindById(Guid.Parse(userId));
            }
            return null;
        }
    }
}
