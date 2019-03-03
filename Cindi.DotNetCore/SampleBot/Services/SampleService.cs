using Cindi.DotNetCore.BotExtensions.Client;
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
        CindiClient _client;
        List<Thread> _testThreads = new List<Thread>();
        ILogger<SampleService> Logger;

        public SampleService(ILogger<SampleService> logger)
        {
            Logger = logger;
            _client = new CindiClient("http://localhost:5021");

            _client.PostStepTemplate(Library.StepTemplateLibrary.First()).GetAwaiter().GetResult();
            _client.PostNewSequenceTemplate(Library.SequenceTemplate).GetAwaiter().GetResult();

            _testThreads.Add(new Thread(async () =>
            {
                while (true)
                {
                    await _client.PostNewSequence(new Cindi.DotNetCore.BotExtensions.ViewModels.SequenceInput()
                    {
                        SequenceTemplateId = Library.SequenceTemplate.Id,
                        Inputs = new Dictionary<string, object>()
                        { }
                    });
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
                    });
                }
            }));

            foreach (var thread in _testThreads)
            {
                thread.Start();
            }

        }
    }
}
