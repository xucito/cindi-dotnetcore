using System;
using System.Collections.Generic;
using System.Text;

namespace Cindi.DotNetCore.BotExtensions.Client
{
    public class UserClientOptions: CindiClientOptions
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
