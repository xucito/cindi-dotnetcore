﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Cindi.DotNetCore.BotExtensions.Exceptions
{
    public class InvalidStepStatusInputException : Exception
    {
        public InvalidStepStatusInputException()
        {
        }

        public InvalidStepStatusInputException(string message)
            : base(message)
        {
        }

        public InvalidStepStatusInputException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
