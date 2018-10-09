using System;
using System.Collections.Generic;
using System.Text;

namespace Cindi.DotNetCore.BotExtensions.Exceptions
{
    public class ConflictingStepTemplateException : Exception
    {
        public ConflictingStepTemplateException()
        {
        }

        public ConflictingStepTemplateException(string message)
            : base(message)
        {
        }

        public ConflictingStepTemplateException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
