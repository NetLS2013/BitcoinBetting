namespace BitcoinBetting.Core.Models.User
{
    public class ForgotPasswordConfirmModel
    {
        public string Email { get; set; }
        public string Code { get; set; }
        public string Password { get; set; }
    }
}