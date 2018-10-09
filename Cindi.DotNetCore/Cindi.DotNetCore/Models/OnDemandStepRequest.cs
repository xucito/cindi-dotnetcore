using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cindi.DotNetCore.BotExtensions.Models
{
    public class OnDemandStepRequest
    {
        /// <summary>
        /// Maps to a Step Definition
        /// </summary>
        [Required]
        public string TemplateId { get; set; }

        /// <summary>
        /// Input for the task, the Input name is the dictionary key and the input value is the Dictionary value
        /// </summary>
        public List<CommonData> Inputs { get; set; }
    }
}
