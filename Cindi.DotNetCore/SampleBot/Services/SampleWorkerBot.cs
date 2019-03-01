using Cindi.Domain.Entities.Steps;
using Cindi.Domain.Utilities;
using Cindi.Domain.ValueObjects;
using Cindi.DotNetCore.BotExtensions;
using Cindi.DotNetCore.BotExtensions.Requests;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace SampleBot.Services
{
    public class SampleWorkerBot: WorkerBotHandler<WorkerBotHandlerOptions>
    {
        public SampleWorkerBot(IOptionsMonitor<WorkerBotHandlerOptions> options, ILoggerFactory logger, UrlEncoder encoder): base(options, logger, encoder)
        {
        }

        public override Task<UpdateStepRequest> HandleStep(Step step)
        {
            var updateRequest = new UpdateStepRequest()
            {
                Id = step.Id
            };
            switch(step.StepTemplateId)
            {
                case "Fibonacci_stepTemplate:0":
                    var result = CalculateFibonacci((Int64)DynamicDataUtility.GetData(step.Inputs, "n-1").Value, (Int64)(DynamicDataUtility.GetData(step.Inputs, "n-2").Value));
                    updateRequest.Outputs = new Dictionary<string, object>() { { "n", result } };
                    updateRequest.Status = StepStatuses.Successful;
                    return Task.FromResult(updateRequest);
            }

            throw new NotImplementedException();
        }

        private Int64 CalculateFibonacci(Int64 firstValue, Int64 secondValue)
        {
            return firstValue + secondValue;
        }
    }
}
