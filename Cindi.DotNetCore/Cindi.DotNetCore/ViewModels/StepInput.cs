using System;
using System.Collections.Generic;
using System.Text;

namespace Cindi.DotNetCore.BotExtensions.ViewModels
{
    public class StepInput
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string StepTemplateId { get; set; }
        public Dictionary<string, object> Inputs { get; set; }
        public List<string> Tests { get; set; }
    }
}
