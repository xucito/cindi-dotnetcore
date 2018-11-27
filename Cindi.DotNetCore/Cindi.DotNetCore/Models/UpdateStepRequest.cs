using System;
using System.Collections.Generic;
using System.Text;

namespace Cindi.DotNetCore.BotExtensions.Models
{
    public class UpdateStepRequest
    {
        public UpdateStepRequest()
        {
            Outputs = new List<CommonData>();
        }

        public UpdateStepRequest(Step step)
        {
            this.Id = step.Id;
            this.Outputs = step.Outputs;
            this.Status = step.Status;
            this.StatusCode = step.StatusCode;
            this.Log = step.Log;
        }

        public int Id { get; set; }

        /// <summary>
        /// Output from task, the output name is the dictionary key and the value is Dictionary value
        /// </summary>
        public List<CommonData> Outputs { get; set; }

        public string Status { get; set; }
        /// <summary>
        /// Combined with Status can be used to evaluate dependencies
        /// </summary>
        public int StatusCode { get; set; }

        public string Log { get; set; }
    }
}
