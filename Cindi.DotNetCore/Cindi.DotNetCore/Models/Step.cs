using Cindi.DotNetCore.BotExtensions.Exceptions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cindi.DotNetCore.BotExtensions.Models
{


    public class Step
    {
        public Step()
        {
            Inputs = new List<CommonData>();
            Outputs = new List<CommonData>();
        }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// The sequence this step belongs to 
        /// </summary>
        public int? SequenceId { get; set; }
        /// <summary>
        /// The template that the sequence is defined by
        /// </summary>
        //public string SequenceTemplateId { get; set; }
        /// <summary>
        /// Used to map to a specific step in a sequence
        /// </summary>
        public int StepRefId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        [Required]
        public TemplateReference StepTemplateReference { get; set; }

        /*
        /// <summary>
        /// List of tasks to be run based on the id of the task and the status of that task
        /// </summary>
        public Dictionary<string, int> Dependencies { get; set; }
        */

        /// <summary>
        /// Input for the task, the Input name is the dictionary key and the input value is the Dictionary value
        /// </summary>
        public List<CommonData> Inputs { get; set; }

        /// <summary>
        /// Output from task, the output name is the dictionary key and the value is Dictionary value
        /// </summary>
        public List<CommonData> Outputs { get; set; }

        public DateTimeOffset CreatedOn { get; set; }

        /// <summary>
        /// Completed is the date the step is moved to a completed queue
        /// </summary>
        public DateTimeOffset Completed { get; set; }

        public string Status { get; set; }

        /// <summary>
        /// Combined with Status can be used to evaluate dependencies
        /// </summary>
        public int StatusCode { get; set; }

        public string Log { get; set; }
    }


    /// <summary>
    /// The status of the task
    /// </summary>
    public class Statuses
    {
        public static string Unassigned { get { return "unassigned"; } }
        public static string Assigned { get { return "assigned"; } }
        public static string Successful { get { return "successful"; } }
        public static string Warning { get { return "warning"; } }
        public static string Error { get { return "error"; } }

        public static bool IsValid(string value)
        {
            if (value == Unassigned ||
                value == Assigned ||
                value == Successful ||
                value == Warning ||
                value == Error)
            {
                return true;
            }
            return false;
        }
    };

}
