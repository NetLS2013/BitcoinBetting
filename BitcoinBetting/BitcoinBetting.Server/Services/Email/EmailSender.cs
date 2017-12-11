using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using BitcoinBetting.Server.Services.Contracts;

namespace BitcoinBetting.Server.Services.Email
{
    public class EmailSender : IEmailSender
    {
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com")
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("yarynam1005@gmail.com", "QWERTY1005"),
                EnableSsl = true
            };
            
            var mailMessage = new MailMessage()
            {
                From = new MailAddress("yarynam1005@gmail.com"),
                To = { email },
                Subject = subject,
                Body = message
            };

            await client.SendMailAsync(mailMessage);
        }
    }
}