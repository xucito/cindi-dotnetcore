using System;
using System.Collections.Generic;
using System.Text;

namespace Cindi.DotNetCore.BotExtensions.Requests
{
    /// <summary>
    /// Used to request a step from a node
    /// </summary>
    public class StepRequest
    {
        /// <summary>
        /// The definitions for which this request can match
        /// </summary>
        public string[] StepTemplateIds { get; set; }
    }
}
