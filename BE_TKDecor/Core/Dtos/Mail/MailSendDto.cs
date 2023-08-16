using System.ComponentModel.DataAnnotations;

namespace BE_TKDecor.Core.Dtos.Mail
{
    public class MailSendDto
    {
        [MaxLength(100)]
        public string Name { get; set; } = null!;
        [MaxLength(100)]
        public string MailSender { get; set; } = null!;
        public string Content { get; set; } = null!;
    }
}
