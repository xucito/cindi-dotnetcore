using Cindi.Domain.Entities.StepTemplates;
using Cindi.Domain.Enums;
using Cindi.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SampleBot
{
    public static class Library
    {
        public static List<StepTemplate> StepTemplateLibrary = new List<StepTemplate>()
        {
            new StepTemplate()
            {
                Id = "Fibonacci_stepTemplate:0",
                InputDefinitions = new Dictionary<string, DynamicDataDescription>(){
                        {"n-2", new DynamicDataDescription(){
                            Type = InputDataTypes.Int
                        } },

                        {"n-1", new DynamicDataDescription(){
                            Type = InputDataTypes.Int
                        } }
                    },
                OutputDefinitions = new Dictionary<string, DynamicDataDescription>()
                    {
                         {"n", new DynamicDataDescription(){
                            Type = InputDataTypes.Int
                        } },
                    }
            }
    };
    }
}
