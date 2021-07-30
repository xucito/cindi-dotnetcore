using System;
using System.Collections.Generic;
using System.Text;

namespace Cindi.DotNetCore.BotExtensions.Requests
{
    public class UpdateStepRequest
    {
        public Guid Id { get; set; }
        public Dictionary<string, object> Outputs { get; set; }
        public string Status { get; set; }
        public int StatusCode { get; set; }
        public string Log { get; set; }
        public string EncryptionKey { get; set; }
    }
}
