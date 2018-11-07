using System;
using System.Collections.Generic;
using System.Text;

namespace Cindi.DotNetCore.BotExtensions.Models
{
    public class StepOutputReference
    {
        /// <summary>
        /// Use -1 for sequence
        /// </summary>
        public int StepRefId { get; set; }

        public string OutputId { get; set; }

        /// <summary>
        /// Will map first available step output based on highest priority logic
        /// </summary>
        public int? Priority { get; set; }
    }
}
