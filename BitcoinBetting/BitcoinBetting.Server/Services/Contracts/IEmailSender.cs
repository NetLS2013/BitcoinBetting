using System.Threading.Tasks;

namespace BitcoinBetting.Server.Services.Contracts
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}