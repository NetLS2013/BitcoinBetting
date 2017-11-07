using System.Threading.Tasks;
using BitcoinBetting.Server.Services.Contracts;

namespace BitcoinBetting.Server.Services.Email
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string message)
        {
            return Task.CompletedTask;
        }
    }
}