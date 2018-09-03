using System;

namespace Kaoiji.consts
{
    public static class LoginExceptions
    {
        public static LoginException INVALID_LOGIN_DATA = new LoginException("Invalid Login Data!");
    }
    public class LoginException : Exception
    {
        public LoginException(string message) : base(message) { }
    }
}
