using BE_TKDecor.Core.Dtos.Mail;
using BE_TKDecor.Core.Mail;
using BE_TKDecor.Core.Response;
using Microsoft.AspNetCore.Mvc;

namespace BE_TKDecor.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailsController : ControllerBase
    {
        private readonly ISendMailService _sendMailService;

        public MailsController(ISendMailService sendMailService)
        {
            _sendMailService = sendMailService;
        }

        // api/Mails/GetComment
        [HttpPost("GetComment")]
        public async Task<IActionResult> GetComment(MailSendDto mailSendDto)
        {
            //set data to send
            MailContent mailContent = new()
            {
                To = "DuyenNP7901@gmail.com",
                Subject = "Mail để đưa ra nhận xét về trang web",
                Body = $"<h4>Được góp ý bởi: ${mailSendDto.MailSender}. Tên khánh hàng: {mailSendDto.Name}</h4>" +
                $"<p>Nội dung: {mailSendDto.Content}</p>"
            };
            // send mail
            await _sendMailService.SendMail(mailContent);

            return Ok(new ApiResponse { Success = true });
        }
    }
}
