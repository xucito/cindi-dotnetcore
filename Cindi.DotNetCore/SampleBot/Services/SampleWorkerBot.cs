using Cindi.DotNetCore.BotExtensions;
using Cindi.DotNetCore.BotExtensions.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using static Cindi.DotNetCore.BotExtensions.Models.CommonData;

namespace SampleBot.Services
{
    public class SampleWorkerBot: WorkerBotHandler<WorkerBotHandlerOptions>
    {
        public SampleWorkerBot(IOptionsMonitor<WorkerBotHandlerOptions> options, ILoggerFactory logger, UrlEncoder encoder): base(options, logger, encoder)
        {
        }

        public override Task<Step> HandleStep(Step step)
        {
            switch(step.TemplateId)
            {
                case "Calculate_Fibonacci_v0":
                    var result = CalculateFibonacci(int.Parse(BotUtility.GetData(step.Inputs, "n-1").Value), int.Parse(BotUtility.GetData(step.Inputs, "n-2").Value));
                    step.Outputs.Add(new CommonData("n", (int)InputDataType.Int, ""+result));
                    step.Status = Statuses.Successful;
                    step.Completed = DateTime.UtcNow;
                    return Task.FromResult(step);
            }

            throw new NotImplementedException();
        }

        private int CalculateFibonacci(int firstValue, int secondValue)
        {
            return firstValue + secondValue;
        }
    }
}
