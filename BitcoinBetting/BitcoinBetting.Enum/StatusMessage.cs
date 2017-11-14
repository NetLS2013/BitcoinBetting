using System;

namespace BitcoinBetting.Enum
{
    public enum StatusMessage
    {
        EmailNotConfirmed = 1,
        UsernameOrPasswordIncorrect = 2,
        ErrorCreatingUser = 3,
        EmailDuplicate = 4,
        EmailNotExist = 5,
        WrongCode = 6,
        ErrorChangePass = 7
    }
}