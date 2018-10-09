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

        /// <summary>
        /// The sequence this step belongs to 
        /// </summary>
        public int? SequenceId { get; set; }
        /// <summary>
        /// The template that the sequence is defined by
        /// </summary>
        public string SequenceTemplateId { get; set; }
        /// <summary>
        /// Used to map to a specific step in a sequence
        /// </summary>
        public int StepRefId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Maps to a Step Definition
        /// </summary>
        public string TemplateId { get; set; }

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

    public class CommonData
    {
        public enum InputDataType { Int, String, Bool, Object }

        public CommonData()
        { }


        public CommonData(string id, int type, string value)
        {
            this.Id = id;
            this.Type = type;

            try
            {
                switch (type)
                {
                    case (int)InputDataType.Bool:
                        var result = Convert.ToBoolean(value);
                        break;
                    case (int)InputDataType.String:
                        break;
                    case (int)InputDataType.Int:
                        var intConversion = int.Parse(value);
                        break;
                    case (int)InputDataType.Object:
                        var jsonConversion = JsonConvert.DeserializeObject(value);
                        break;
                }
                Value = value;
            }
            catch (Exception e)
            {
                throw new InvalidInputValueException(e.Message);
            }
        }

        public string Id { get; set; }
        public int Type { get; set; }
        public string Value { get; set; }
    }
}
