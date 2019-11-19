using Cindi.DotNetCore.BotExtensions;
using Cindi.DotNetCore.BotExtensions.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;

namespace SampleBot.Services
{
    public class SampleService
    {
        UserClient _client;
        List<Thread> _testThreads = new List<Thread>();
        ILogger<SampleService> Logger;
        Random rand = new Random();
        ILoggerFactory loggerFactory;
        IConfiguration configuration;
        public SampleService(ILoggerFactory loggerFactory, ILogger<SampleService> logger, IConfiguration configuration)
        {
            this.loggerFactory = loggerFactory;
            this.configuration = configuration;
            Logger = logger;
            var username = configuration.GetValue<string>("TestUser:username");
            var password = configuration.GetValue<string>("TestUser:password");
            _client = new UserClient(configuration.GetValue<string>("cindiurl"), username, password);

            // var test = _client.GetSteps(username, password).GetAwaiter().GetResult();
            // var sampleStep = _client.GetStep(new Guid("389a9745-5965-48ac-aedd-03496b661b68"), username, password).GetAwaiter().GetResult();
            _client.PostStepTemplate(new Cindi.DotNetCore.BotExtensions.Requests.NewStepTemplateRequest(Library.StepTemplateLibrary.First()), username, password).GetAwaiter().GetResult();
            _client.PostNewWorkflowTemplate(Library.WorkflowTemplate, username, password).GetAwaiter().GetResult();
            _client.PostNewWorkflowTemplate(Library.WorkflowTemplate2, username, password).GetAwaiter().GetResult();


             _testThreads.Add(new Thread(async () =>
             {
                 while (true)
                 {
                     var result = await _client.PostNewWorkflow(new Cindi.DotNetCore.BotExtensions.ViewModels.WorkflowInput()
                     {
                         WorkflowTemplateId = Library.WorkflowTemplate.ReferenceId,
                         Inputs = new Dictionary<string, object>()
                         { }
                     }, username, password);
                     Thread.Sleep(rand.Next(0, 10000));
                 }
             }));
             _testThreads.Add(new Thread(async () =>
             {
                 while (true)
                 {
                     var result = await _client.PostNewStep(new Cindi.DotNetCore.BotExtensions.ViewModels.StepInput()
                     {
                         StepTemplateId = Library.SecretStepTemplate.ReferenceId,
                         Inputs = new Dictionary<string, object>()
                     {
                         {"secret", "This is a test" }
                     }
                     }, username, password);

                     Thread.Sleep(rand.Next(0, 10000));
                 }
             }));

            foreach (var thread in _testThreads)
            {
                thread.Start();
            }

            CheckConcurrency("admin", "PleaseChangeMe");

        }

        public void CheckConcurrency(string username, string password)
        {
            var result = _client.PostNewStep(new Cindi.DotNetCore.BotExtensions.ViewModels.StepInput()
            {
                StepTemplateId = Library.SecretStepTemplate.ReferenceId,
                Inputs = new Dictionary<string, object>()
                    {
                        {"secret", "This is a test" }
                    }
            }, username, password).GetAwaiter().GetResult();

            var workers = new List<SampleWorkerBot>();
            var numberOfTestedWorkers = 10;
            for (var i = 0; i < numberOfTestedWorkers; i++)
            {
                workers.Add(new SampleWorkerBot(new WorkerBotHandlerOptions()
                {
                    NodeURL = configuration.GetValue<string>("cindiurl"),
                    SleepTime = 100,
                    StepTemplateLibrary = new List<Cindi.Domain.Entities.StepTemplates.StepTemplate>() {
                    Library.SecretStepTemplate,
                    Library.StepTemplate
                },
                    AutoStart = false
                }, loggerFactory, UrlEncoder.Default));
            }

            int numberOfStepsFound = 0;
            ConcurrentBag<Guid> stepAssigned = new ConcurrentBag<Guid>();
            Parallel.ForEach(workers, worker =>
            {
                var getTask = worker.GetNextStep().GetAwaiter().GetResult();
                if (getTask != null)
                {
                    Interlocked.Increment(ref numberOfStepsFound);
                    stepAssigned.Add(getTask.Id);
                }
            });

            Console.WriteLine("Number of total steps picked up: " + numberOfStepsFound);
        }
    }
}
