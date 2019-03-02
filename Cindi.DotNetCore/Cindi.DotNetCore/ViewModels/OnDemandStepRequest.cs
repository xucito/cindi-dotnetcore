using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cindi.DotNetCore.BotExtensions.Requests
{
    public class OnDemandStepRequest
    {
        [Required]
        public string StepTemplateId { get; set; }
        public Dictionary<string, object> Inputs { get; set; }
    }
}
