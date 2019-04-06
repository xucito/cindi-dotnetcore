using Cindi.DotNetCore.BotExtensions.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SampleBot.Services
{
    public class SampleService
    {
        UserClient _client;
        List<Thread> _testThreads = new List<Thread>();
        ILogger<SampleService> Logger;

        public SampleService(ILogger<SampleService> logger, IConfiguration configuration)
        {
            Logger = logger;
            var username = configuration.GetValue<string>("TestUser:username");
            var password = configuration.GetValue<string>("TestUser:password");
            _client = new UserClient("http://localhost:5021", username, password);

            _client.PostStepTemplate(new Cindi.DotNetCore.BotExtensions.Requests.NewStepTemplateRequest(Library.StepTemplateLibrary.First()), username, password).GetAwaiter().GetResult();
            _client.PostNewSequenceTemplate(Library.SequenceTemplate, username, password).GetAwaiter().GetResult();

            _testThreads.Add(new Thread(async () =>
            {
                while (true)
                {
                    await _client.PostNewSequence(new Cindi.DotNetCore.BotExtensions.ViewModels.SequenceInput()
                    {
                        SequenceTemplateId = Library.SequenceTemplate.Id,
                        Inputs = new Dictionary<string, object>()
                        { }
                    }, username, password);
                }
            }));

            _testThreads.Add(new Thread(async () =>
            {
                while (true)
                {
                    await _client.PostNewStep(new Cindi.DotNetCore.BotExtensions.ViewModels.StepInput()
                    {
                        StepTemplateId = Library.StepTemplate.Id,
                        Inputs = new Dictionary<string, object>()
                    {
                        {"n-1", 1 },
                        {"n-2",1 }
                    }
                    }, username, password);
                }
            }));

            foreach (var thread in _testThreads)
            {
                thread.Start();
            }

        }
    }
}
