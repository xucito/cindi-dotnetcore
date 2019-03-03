using Cindi.Domain.Entities.SequencesTemplates;
using Cindi.Domain.Entities.Steps;
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


        public static StepTemplate StepTemplate = new StepTemplate()
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
        };

        public static List<StepTemplate> StepTemplateLibrary = new List<StepTemplate>()
        {
            StepTemplate
        };

        public static SequenceTemplate SequenceTemplate = new SequenceTemplate()
        {
            Id = "SimpleSequence:1",
            LogicBlocks = new List<LogicBlock>()
            {
                new LogicBlock()
                {
                    Id = 0,
                    Condition = "OR",
                    PrerequisiteSteps = new List<PrerequisiteStep>
                    {
                    },
                    SubsequentSteps = new List<SubsequentStep> {
                         new SubsequentStep(){
                             StepTemplateId =StepTemplate.Id,
                             StepRefId = 0,
                                      Mappings = new List<Mapping>(){
                                      new Mapping()
                                       {
                                           DefaultValue = new DefaultValue(){
                                               Value = 1
                                           },
                                           StepInputId = "n-1"
                                       },
                                      new Mapping(){
                                           DefaultValue = new DefaultValue(){
                                               Value = 1
                                           },
                                           StepInputId = "n-2"
                                       }
                                   }
                         } }
                },
                new LogicBlock()
                {
                    Id = 1,
                    Condition = "AND",
                    PrerequisiteSteps = new List<PrerequisiteStep>
                    {
                        new PrerequisiteStep()
                        {
                            StepRefId = 0,
                            Status = StepStatuses.Successful,
                            StatusCode = 0
                        }
                    },
                    SubsequentSteps = new List<SubsequentStep> {
                         new SubsequentStep(){
                             StepTemplateId =StepTemplate.Id,
                             StepRefId = 1,
                                      Mappings = new List<Mapping>(){
                                      new Mapping()
                                       {
                                          OutputReferences = new StepOutputReference[]
                                          {
                                              new StepOutputReference()
                                              {
                                                  StepRefId = 0,
                                                  OutputId = "n"
                                              }
                                          },
                                           DefaultValue = new DefaultValue(){
                                               Value = 1
                                           },
                                           StepInputId = "n-1"
                                       },
                                      new Mapping(){
                                            OutputReferences = new StepOutputReference[]
                                            {
                                                new StepOutputReference()
                                                {
                                                    StepRefId = 0,
                                                    OutputId = "n"
                                                }
                                            },
                                           DefaultValue = new DefaultValue(){
                                               Value = 1
                                           },
                                           StepInputId = "n-2"
                                       }
                                   }
                         } }
                }
            }
        };
    }
}
