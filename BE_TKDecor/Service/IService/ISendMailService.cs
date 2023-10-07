using BE_TKDecor.Core.Mail;

namespace BE_TKDecor.Service.IService
{
    public interface ISendMailService
    {
        Task SendMail(MailContent mailContent);

        Task SendEmailAsync(string email, string subject, string htmlMessage);
    }
}
