using System.Threading.Tasks;
using BitcoinBetting.Server.Services.Contracts;
using MailChimp.Net;
using MailChimp.Net.Models;

namespace BitcoinBetting.Server.Services.MailChimp
{
    public class MailChimpSender : IMailChimpSender
    {
        private readonly MailChimpManager mailChimpManager;

        public MailChimpSender()
        {
            this.mailChimpManager = new MailChimpManager("d7497ad83b0883fd55f63f2df3df3d11-us17");
        }

        public async Task AddUserAsync(string email, string firstName, string lastName)
        {
            var member = new Member
            {
                EmailAddress = email, StatusIfNew = Status.Subscribed
            };
            
            member.MergeFields.Add("FNAME", firstName);
            member.MergeFields.Add("LNAME", lastName);
            
            await mailChimpManager.Members.AddOrUpdateAsync("18059db0ca", member);
        }
    }
}