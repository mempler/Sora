using System;

namespace Kaoiji.consts
{
    public class LoginExceptions : Exception
    {
        public static LoginExceptions INVALID_LOGIN_DATA = new LoginExceptions("Invalid Login Data!");

        public LoginExceptions(string message) : base(message) { }
    }
}
