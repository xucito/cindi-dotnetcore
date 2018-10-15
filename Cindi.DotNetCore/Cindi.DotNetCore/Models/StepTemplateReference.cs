using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Cindi.DotNetCore.BotExtensions.Models
{
    public class StepTemplateReference
    {
        public string Name { get; set; }
        public int Version { get; set; }

        [JsonIgnore]
        public string TemplateId { get { return Name + ":" + Version; } }
    }
}
