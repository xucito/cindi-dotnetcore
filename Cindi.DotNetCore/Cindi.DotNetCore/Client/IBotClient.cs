using System;
using System.Threading.Tasks;
using Cindi.Domain.Entities.Steps;
using Cindi.Domain.Entities.WorkflowsTemplates;
using Cindi.DotNetCore.BotExtensions.Models;
using Cindi.DotNetCore.BotExtensions.Requests;
using Cindi.DotNetCore.BotExtensions.ViewModels;

namespace Cindi.DotNetCore.BotExtensions.Client
{
    public interface IBotClient
    {
        Task<string> AddStepLog(Guid stepId, string log, string idToken);
        void BackOff(int attempt, string command, Exception e, int maxAttempts);
        Task<string> CompleteStep(UpdateStepRequest request, string idToken);
        Task<NextStep> GetNextStep(StepRequest request, string idToken);
        Task<string> PostNewStep(StepInput stepInput, string idToken);
        Task<string> PostNewWorkflow(WorkflowInput input, string idToken);
        Task<string> PostNewWorkflowTemplate(WorkflowTemplate WorkflowTemplate, string idToken);
        Task<string> PostStepTemplate(NewStepTemplateRequest stepTemplate, string idToken);
        Task<NewBotKeyResult> RegisterBot(string name, string rsaPublicKey);
        void SetIdToken(string botId);
    }
}