using Cindi.Domain.Entities.Steps;
using Cindi.Domain.Entities.StepTemplates;
using Cindi.Domain.Entities.WorkflowsTemplates;
using Cindi.Domain.Entities.WorkflowTemplates.Conditions;
using Cindi.Domain.Entities.WorkflowTemplates.ValueObjects;
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
            ReferenceId = "Fibonacci_stepTemplate:0",
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

        public static StepTemplate SecretStepTemplate = new StepTemplate()
        {
            ReferenceId = "Pass_Password:0",
            InputDefinitions = new Dictionary<string, DynamicDataDescription>(){

                        {"Secret", new DynamicDataDescription(){
                            Type = InputDataTypes.Secret,
                            Description = ""
                        }}
                    },
            OutputDefinitions = new Dictionary<string, DynamicDataDescription>()
                    {
                         {"secret", new DynamicDataDescription(){
                            Type = InputDataTypes.Secret,
                            Description = ""
                        }},
                    }
        };

        public static List<StepTemplate> StepTemplateLibrary = new List<StepTemplate>()
        {
            StepTemplate,
            SecretStepTemplate
        };

        public static WorkflowTemplate WorkflowTemplate = new WorkflowTemplate()
        {
            ReferenceId = "SimpleWorkflow:1",
            LogicBlocks = new Dictionary<string, LogicBlock>()
            {
                {"0", new LogicBlock()
                {
                    Dependencies = new ConditionGroup
                    {
                    },
                    SubsequentSteps = new Dictionary<string, SubsequentStep> {
                        {
                        "0",
                         new SubsequentStep(){
                             StepTemplateId = StepTemplate.ReferenceId,
                                      Mappings = new Dictionary<string, Mapping>(){
                                          {"n-1",
                                      new Mapping()
                                       {
                                           DefaultValue = new DefaultValue(){
                                               Value = 1
                                           }
                                       }
                                          },
                                          {"n-2",
                                      new Mapping(){
                                           DefaultValue = new DefaultValue(){
                                               Value = 1
                                           }
                                       }
                                   }
                                      }
                         }
                         } }
                }
                },
                {"1", new LogicBlock()
                {
                    Dependencies = new ConditionGroup
                    {
                        Operator = OperatorStatements.AND,
                        Conditions = new Dictionary<string, Condition>()
                        {
                            { "0",
                           new StepStatusCondition()
                           {
                                                           StepName = "0",
                            Status = StepStatuses.Successful,
                            StatusCode = 0
                           }
                            }
                        }
                    },
                    SubsequentSteps = new Dictionary<string, SubsequentStep> {
                        { "1",
                         new SubsequentStep(){
                             StepTemplateId =StepTemplate.ReferenceId,
                                      Mappings = new Dictionary<string, Mapping>(){
                                      {"n-1",
                                      new Mapping()
                                       {
                                          OutputReferences = new StepOutputReference[]
                                          {
                                              new StepOutputReference()
                                              {
                                                  StepName = "0",
                                                  OutputId = "n"
                                              }
                                          },
                                           DefaultValue = new DefaultValue(){
                                               Value = 1
                                           }
                                       }
                                          },
                                          {
                                              "n-2",
                                      new Mapping(){
                                            OutputReferences = new StepOutputReference[]
                                            {
                                                new StepOutputReference()
                                                {
                                                    StepName = "0",
                                                    OutputId = "n"
                                                }
                                            },
                                           DefaultValue = new DefaultValue(){
                                               Value = 1
                                           }
                                       }
                                      }
                                      }
                                   }
                         } }
                }
                }
            }
        };

        public static WorkflowTemplate ConcurrencyTest = new WorkflowTemplate()
        {
            ReferenceId = "ConcurrencyTest:1",
            InputDefinitions = new Dictionary<string, DynamicDataDescription>()
            {
                {"n-1", new DynamicDataDescription()
                {
                    Type = InputDataTypes.Int
                }
                },
                { "n-2", new DynamicDataDescription()
                {
                    Type = InputDataTypes.Int
                }
                }
            },
            LogicBlocks = new Dictionary<string, LogicBlock>()
            {
                { "0", new LogicBlock()
                {
                    Dependencies = new ConditionGroup
                    {
                    },
                    SubsequentSteps = new Dictionary<string, SubsequentStep> {
                        { "0",
                         new SubsequentStep(){
                             StepTemplateId = StepTemplate.ReferenceId,
                                      Mappings = new Dictionary<string, Mapping>(){
                                          {
                                               "n-1",
                                      new Mapping()
                                       {
                                          OutputReferences = new StepOutputReference[]
                                          {
                                              new StepOutputReference()
                                              {
                                                  StepName = ReservedValues.WorkflowStartStepName,
                                                  OutputId = "n-1"
                                              }
                                          },
                                           DefaultValue = new DefaultValue(){
                                               Value = 1
                                           }
                                       } },
                                      {"n-2",
                                      new Mapping(){
                                           OutputReferences = new StepOutputReference[]
                                          {
                                              new StepOutputReference()
                                              {
                                                  StepName = ReservedValues.WorkflowStartStepName,
                                                  OutputId = "n-2"
                                              }
                                          },
                                           DefaultValue = new DefaultValue(){
                                               Value = 1
                                           }
                                       }
                                   }
                         } }
                        }
                    }
                }
                },
                { "1", new LogicBlock()
                {
                    Dependencies = new ConditionGroup
                    {
                    },
                    SubsequentSteps = new Dictionary<string, SubsequentStep> {
                        { "1",
                         new SubsequentStep(){
                             StepTemplateId = StepTemplate.ReferenceId,
                                      Mappings = new Dictionary<string, Mapping>(){
                                          {
                                               "n-1",
                                      new Mapping()
                                       {
                                          OutputReferences = new StepOutputReference[]
                                          {
                                              new StepOutputReference()
                                              {
                                                  StepName = ReservedValues.WorkflowStartStepName,
                                                  OutputId = "n-1"
                                              }
                                          },
                                           DefaultValue = new DefaultValue(){
                                               Value = 1
                                           }
                                       } },
                                      {"n-2",
                                      new Mapping(){
                                           OutputReferences = new StepOutputReference[]
                                          {
                                              new StepOutputReference()
                                              {
                                                  StepName = ReservedValues.WorkflowStartStepName,
                                                  OutputId = "n-2"
                                              }
                                          },
                                           DefaultValue = new DefaultValue(){
                                               Value = 1
                                           }
                                       }
                                   }
                         } }
                        }
                    }
                }
                },
                    {"2", new LogicBlock()
                {
                    Dependencies = new ConditionGroup
                    {
                        Operator = OperatorStatements.AND,
                        Conditions = new Dictionary<string, Condition>()
                        {
                            { "0",
                           new StepStatusCondition()
                           {
                                                           StepName = "0",
                            Status = StepStatuses.Successful,
                            StatusCode = 0
                           }
                            }
                        }
                    },
                    SubsequentSteps = new Dictionary<string, SubsequentStep>{
                        { "2",
                         new SubsequentStep(){
                             StepTemplateId =StepTemplate.ReferenceId,
                                      Mappings = new Dictionary<string, Mapping>(){
                                          {"n-1",
                                      new Mapping()
                                       {
                                          OutputReferences = new StepOutputReference[]
                                          {
                                              new StepOutputReference()
                                              {
                                                  StepName = "0",
                                                  OutputId = "n"
                                              }
                                          },
                                           DefaultValue = new DefaultValue(){
                                               Value = 1
                                           }
                                       } },
                                      {"n-2",
                                      new Mapping(){
                                            OutputReferences = new StepOutputReference[]
                                            {
                                                new StepOutputReference()
                                                {
                                                    StepName = "0",
                                                    OutputId = "n"
                                                }
                                            },
                                           DefaultValue = new DefaultValue(){
                                               Value = 1
                                           }
                                       }
                                   }
                         } }
                    }
                }
                    }
                }
            }
        };

        public static WorkflowTemplate WorkflowTemplate2 = new WorkflowTemplate()
        {
            ReferenceId = "SimpleWorkflowWithInputs:1",
            InputDefinitions = new Dictionary<string, DynamicDataDescription>()
            {
                {"n-1", new DynamicDataDescription()
                {
                    Type = InputDataTypes.Int
                }
                },

                { "n-2", new DynamicDataDescription()
                {
                    Type = InputDataTypes.Int
                }
                }
        },
            LogicBlocks = new Dictionary<string, LogicBlock>()
            {
                { "0", new LogicBlock()
                {
                    Dependencies = new ConditionGroup
                    {
                    },
                    SubsequentSteps = new Dictionary<string, SubsequentStep> {
                        { "0",
                         new SubsequentStep(){
                             StepTemplateId = StepTemplate.ReferenceId,
                                      Mappings = new Dictionary<string, Mapping>(){
                                          {
                                               "n-1",
                                      new Mapping()
                                       {
                                          OutputReferences = new StepOutputReference[]
                                          {
                                              new StepOutputReference()
                                              {
                                                  StepName = ReservedValues.WorkflowStartStepName,
                                                  OutputId = "n-1"
                                              }
                                          },
                                           DefaultValue = new DefaultValue(){
                                               Value = 1
                                           }
                                       } },
                                      {"n-2",
                                      new Mapping(){
                                           OutputReferences = new StepOutputReference[]
                                          {
                                              new StepOutputReference()
                                              {
                                                  StepName = ReservedValues.WorkflowStartStepName,
                                                  OutputId = "n-2"
                                              }
                                          },
                                           DefaultValue = new DefaultValue(){
                                               Value = 1
                                           }
                                       }
                                   }
                         } }
                        }
                    }
                }
                },
                    {"1", new LogicBlock()
                {
                    Dependencies = new ConditionGroup
                    {
                        Operator = OperatorStatements.AND,
                        Conditions = new Dictionary<string, Condition>()
                        {
                            { "0",
                           new StepStatusCondition()
                           {
                                                           StepName = "0",
                            Status = StepStatuses.Successful,
                            StatusCode = 0
                           }
                            }
                        }
                    },
                    SubsequentSteps = new Dictionary<string, SubsequentStep>{
                        { "1",
                         new SubsequentStep(){
                             StepTemplateId =StepTemplate.ReferenceId,
                                      Mappings = new Dictionary<string, Mapping>(){
                                          {"n-1",
                                      new Mapping()
                                       {
                                          OutputReferences = new StepOutputReference[]
                                          {
                                              new StepOutputReference()
                                              {
                                                  StepName = "0",
                                                  OutputId = "n"
                                              }
                                          },
                                           DefaultValue = new DefaultValue(){
                                               Value = 1
                                           }
                                       } },
                                      {"n-2",
                                      new Mapping(){
                                            OutputReferences = new StepOutputReference[]
                                            {
                                                new StepOutputReference()
                                                {
                                                    StepName = "0",
                                                    OutputId = "n"
                                                }
                                            },
                                           DefaultValue = new DefaultValue(){
                                               Value = 1
                                           }
                                       }
                                   }
                         } }
                    }
                }
                    }
                }
            }
        };
    }
}
