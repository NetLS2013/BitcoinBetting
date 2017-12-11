using BitcoinBetting.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace BitcoinBetting.Core.Models
{
    public static class ResponseMessage
    {
        public static Dictionary<StatusMessage, string> Messages = new Dictionary<StatusMessage, string>
        {
            { StatusMessage.EmailNotConfirmed, "Confim email" },
            { StatusMessage.UsernameOrPasswordIncorrect, "Email or password is incorect" },
            { StatusMessage.ErrorCreatingUser, "Cannot create user, try again" },
            { StatusMessage.EmailDuplicate, "User with this email already exists" },
            { StatusMessage.EmailNotExist, "Account not found, please check the e-mail" },
            { StatusMessage.WrongCode, "The number you entered doesn’t match your code. Please try again" }
        };
    }
}
