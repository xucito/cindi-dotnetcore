using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Cindi.DotNetCore.BotExtensions.Exceptions;

namespace Cindi.DotNetCore.BotExtensions.Models
{
    public class SequenceTemplate
    {

        public SequenceTemplate()
        {
            this.LogicBlocks = new List<LogicBlock>();
            this.StartingMapping = new List<Mapping>();
        }
        /// <summary>
        /// Name of definition
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Version of the definition
        /// </summary>
        public int Version { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string SequenceTemplateId { get { return Name + "_v" + Version; } }
        public List<LogicBlock> LogicBlocks { get; set; }
        public string StartingStepTemplateId { get; set; }
        public List<Mapping> StartingMapping { get; set; }
        /// <summary>
        /// Input from dependency with input name is the dictionary key and the type as the Dictionary value
        /// </summary>
        public Dictionary<string, string> InputDefinitions { get; set; }
    }

    public class LogicBlock
    {
        public LogicBlock()
        {
            PrerequisiteSteps = new List<PrerequisiteStep>();
            SubsequentSteps = new List<SubsequentStep>();
        }

        public int Id { get; set; }
        public string Condition { get; set; }
        public new List<PrerequisiteStep> PrerequisiteSteps { get; set; }
        public new List<SubsequentStep> SubsequentSteps { get; set; }
    }
    /*
    public class Dependency
    {
        public int Id { get; set; }
        /// <summary>
        /// AND or OR
        /// </summary>
        public string Condition { get; set; }
        /// <summary>
        /// Key is the output from the Step, value is the input id for which it is mapped to, type must match
        /// </summary>
        public Dictionary<int, int> OutputMappings { get; set; }
        public List<StatusId> Status { get; set; }
    }*/

    public class Logic
    {
        public static string AND { get { return "AND"; } }
        public static string OR { get { return "OR"; } }
    }

    public class PrerequisiteStep
    {
        /// <summary>
        /// Unique Id of the Step defined within the sequence
        /// </summary>
        public int StepRefId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TemplateId { get; set; } 

        private string _status { get; set; }

        public string Status
        {
            get { return _status; }
            set
            {
                if (Statuses.IsValid(value))
                {
                    _status = value;
                }
                else
                {
                    throw new InvalidStepStatusInputException();
                }

            }
        }

        public int StatusCode { get; set; }
    }

    public class SubsequentStep
    {
        public SubsequentStep()
        {
            Mappings = new List<Mapping>();
        }

        public string TemplateId { get; set; }
        public int StepRefId { get; set; }
        public List<Mapping> Mappings { get; set; }
    }

    public class Mapping
    {
        /// <summary>
        /// The reference Id of the Step for this mapping
        /// </summary>
        public int StepRefId { get; set; }
        /// <summary>
        /// Output definition value used to map
        /// </summary>
        public string PrerequisiteOutputId { get; set; }
        /// <summary>
        /// The field that the Step is mapped to
        /// </summary>
        public string StepInputId { get; set; }
    }

    public class SequenceStatuses
    {
        public static string Started { get { return "started"; } }
        public static string Successful { get { return "successful"; } }
        public static string Warning { get { return "warning"; } }
        public static string Error { get { return "error"; } }

        public static bool IsValid(string value)
        {
            if (value == Started ||
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
