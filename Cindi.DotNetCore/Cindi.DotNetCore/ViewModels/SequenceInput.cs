using System;
using System.Collections.Generic;
using System.Text;

namespace Cindi.DotNetCore.BotExtensions.ViewModels
{
    public class SequenceInput
    {
        public string Name { get; set; }
        public string SequenceTemplateId { get; set; }
        public Dictionary<string, object> Inputs { get; set; }
    }
}
