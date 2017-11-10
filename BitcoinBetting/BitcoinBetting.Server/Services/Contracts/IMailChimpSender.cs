using System.Threading.Tasks;

namespace BitcoinBetting.Server.Services.Contracts
{
    public interface IMailChimpSender
    {
        Task AddUserAsync(string email, string firstName, string lastName);
    }
}