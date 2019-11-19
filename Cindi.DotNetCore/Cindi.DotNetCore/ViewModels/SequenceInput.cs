using System;
using System.Collections.Generic;
using System.Text;

namespace Cindi.DotNetCore.BotExtensions.ViewModels
{
    public class WorkflowInput
    {
        public string Name { get; set; }
        public string WorkflowTemplateId { get; set; }
        public Dictionary<string, object> Inputs { get; set; }
    }
}
