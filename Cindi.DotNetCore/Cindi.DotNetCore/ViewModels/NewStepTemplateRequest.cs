using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Cindi.Domain.ValueObjects;

namespace Cindi.DotNetCore.BotExtensions.Requests
{
    public class NewStepTemplateRequest
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Version { get; set; }
        public string Id { get { return Name + ":" + Version; } }
        public bool AllowDynamicInputs = false;
        public Dictionary<string, DynamicDataDescription> InputDefinitions { get; set; }
        public Dictionary<string, DynamicDataDescription> OutputDefinitions { get; set; }
    }
}
