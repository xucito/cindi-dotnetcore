using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cindi.DotNetCore.BotExtensions.Models
{
    public class NewStepTemplateRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public int Version { get; set; }
        public bool AllowDynamicInputs = false;
        public Dictionary<string, DataDescription> InputDefinitions { get; set; }
        public Dictionary<string, DataDescription> OutputDefinitions { get; set; }

        public NewStepTemplateRequest(StepTemplate template)
        {
            this.Name = template.Name;
            this.Version = template.Version;
            this.AllowDynamicInputs = template.AllowDynamicInputs;
            this.InputDefinitions = template.InputDefinitions;
            this.OutputDefinitions = template.OutputDefinitions;
        }
    }
}
