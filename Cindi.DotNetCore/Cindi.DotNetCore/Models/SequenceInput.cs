using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cindi.DotNetCore.BotExtensions.Models
{
    public class SequenceInput
    {
        public string Name { get; set; }
        public TemplateReference SequenceTemplateReference { get; set; }
        public List<CommonData> InputData { get; set; }
    }
}
