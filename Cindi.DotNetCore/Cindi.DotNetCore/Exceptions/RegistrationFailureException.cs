using System;
using System.Collections.Generic;
using System.Text;

namespace Cindi.DotNetCore.BotExtensions.Exceptions
{
    public class RegistrationFailureException : Exception
    {
        public RegistrationFailureException()
        {
        }

        public RegistrationFailureException(string message)
            : base(message)
        {
        }

        public RegistrationFailureException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
