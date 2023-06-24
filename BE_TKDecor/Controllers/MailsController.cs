using BE_TKDecor.Core.Dtos.Mail;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Utility.Mail;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

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
                Subject = "Mail to give comments about the website",
                Body = $"<h4>Is sent from: ${mailSendDto.MailSender}. Customer name: {mailSendDto.Name}</h4>" +
                $"<p>Content: {mailSendDto.Content}</p>"
            };
            // send mail
            await _sendMailService.SendMail(mailContent);

            return NoContent();
        }
    }
}
