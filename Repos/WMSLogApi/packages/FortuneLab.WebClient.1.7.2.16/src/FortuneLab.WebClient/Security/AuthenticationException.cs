using System;

namespace FortuneLab.WebClient.Security
{
    public class AuthenticationException : Exception
    {
        public AuthenticationException(string message)
            : base(message)
        {
        }

        public AuthenticationException(string message, Exception e)
            : base(message, e)
        {
        }
    }

    public class WrongTicketException : AuthenticationException
    {
        public WrongTicketException(string message)
            : base(message)
        {
        }

        public WrongTicketException(string message, Exception e)
            : base(message, e)
        {
        }
    }
}