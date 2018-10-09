using Cindi.DotNetCore.BotExtensions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Cindi.DotNetCore.BotExtensions.Models.CommonData;

namespace SampleBot
{
    public static class Library
    {
        public static List<StepTemplate> StepTemplateLibrary = new List<StepTemplate>()
        {
            new StepTemplate()
            {
                Name = "Calculate_Fibonacci",
                Version = 0,
                InputDefinitions = new Dictionary<string, string>(){
                        {"n-2", "" + (int)InputDataType.Int},
                        {"n-1", "" + (int)InputDataType.Int}
                    },
                OutputDefinitions = new Dictionary<string, string>()
                    {
                         {"n", "" + (int)InputDataType.Int},
                    }
            }
    };
    }
}
