using Cindi.Domain.Entities.Steps;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cindi.DotNetCore.BotExtensions.ViewModels
{
    public class NextStep
    {
        public Step Step { get; set; }
        public string EncryptionKey { get; set; }
    }
}
