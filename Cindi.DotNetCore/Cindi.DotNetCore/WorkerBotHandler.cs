using Cindi.DotNetCore.BotExtensions.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Cindi.DotNetCore.BotExtensions.Exceptions;

namespace Cindi.DotNetCore.BotExtensions
{
    public abstract class WorkerBotHandler<TOptions> where TOptions : WorkerBotHandlerOptions
    {
        private readonly string nodeUrl;
        private HttpClient _client;
        Thread serviceThread;
        private bool started = false;
        private object threadLocker = new Object();
        private int waitTime = 1000;
        private object waitTimeLocker = new Object();
        private bool _hasValidHttpClient = false;
        protected ILogger Logger { get; }
        protected UrlEncoder UrlEncoder { get; }
        public TOptions Options { get; }

        public WorkerBotHandler(IOptionsMonitor<TOptions> options, ILoggerFactory logger, UrlEncoder encoder)
        {
            if (options.CurrentValue.NodeURL != null && options.CurrentValue.NodeURL != "")
            {
                this.nodeUrl = options.CurrentValue.NodeURL;
                _client = new HttpClient();
                _client.BaseAddress = new Uri(this.nodeUrl + "/api");
                _hasValidHttpClient = true;
            }

            Options = options.CurrentValue;

            waitTime = options.CurrentValue.SleepTime;

            // Register the step template
            foreach (var template in options.CurrentValue.StepTemplateLibrary)
            {
                // Queue all the templates for registration
                QueueTemplateForRegistration(template);
            }

            Logger = logger.CreateLogger(this.GetType().FullName);

            // Initiate the registration of all templates and run loop if valid
            StartWorking();
        }


        /// <summary>
        /// List of templates this bot can run against
        /// </summary>
        public List<StepTemplate> RegisteredTemplates = new List<StepTemplate>();

        public async void StartWorking()
        {
            await RegisterAllTemplatesAsync();

            if (_hasValidHttpClient)
            {
                Run();
            }
        }

        /// <summary>
        /// Register all templates for processing
        /// </summary>
        /// <returns></returns>
        public async Task<bool> RegisterAllTemplatesAsync()
        {
            foreach (var template in RegisteredTemplates)
            {
                if (_hasValidHttpClient)
                {
                    try
                    {
                        await RegisterTemplateAsync(template);
                    }
                    catch (Exception e)
                    {
                        Logger.LogWarning(e.Message);
                    }
                }
                else
                {
                    Logger.LogWarning("Could not register template " + template.TemplateId() + " as there is no valid httpClient.");
                }
            }
            return true;
        }

        public void QueueTemplateForRegistration(StepTemplate stepTemplate)
        {
            var foundTemplateCount = RegisteredTemplates.Where(rt => rt.TemplateId() == stepTemplate.TemplateId()).Count();

            if (foundTemplateCount == 0)
            {
                RegisteredTemplates.Add(stepTemplate);
            }
            else if (foundTemplateCount == 1)
            {
                throw new StepTemplateDuplicateFoundException();
            }
            else
            {
                throw new StepTemplateNotFoundException();
            }
        }

        private async Task<bool> RegisterTemplateAsync(StepTemplate stepTemplate)
        {
            var result = await _client.PostAsync(_client.BaseAddress + "/StepTemplates", new StringContent(JsonConvert.SerializeObject(stepTemplate), Encoding.UTF8, "application/json"));
            if (result.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                throw new Exception("Error adding template");
            }
        }

        public async void Run()
        {
            serviceThread = new Thread(BotLoop);

            started = true;

            // Register all queued the template
            await RegisterAllTemplatesAsync();

            try
            {
                serviceThread.Start();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Used for running the main loop
        /// </summary>
        public async void BotLoop()
        {
            var stopWatch = new Stopwatch();
            while (started)
            {
                Console.WriteLine("Starting new Thread");

                Step nextStep = await GetNextStep();

                if (nextStep != null)
                {
                    Console.WriteLine("Processing step " + nextStep.Id);
                    stopWatch.Start();

                    var processResult = await ProcessStep(nextStep);

                    await _client.PutAsync(_client.BaseAddress + "/Steps/" + nextStep.Id, new StringContent(JsonConvert.SerializeObject(processResult), Encoding.UTF8, "application/json"));

                    stopWatch.Stop();
                    Console.WriteLine("Completed Service Loop took approximately " + stopWatch.ElapsedMilliseconds / 1000 + "secs");
                }
                else
                {
                    Console.WriteLine("No step found");
                }

                lock (waitTimeLocker)
                {
                    Console.WriteLine("Sleeping for " + waitTime + "ms");
                    Thread.Sleep(waitTime);
                }
            }
        }

        /// <summary>
        /// Get the next step based on defintions acceptable
        /// </summary>
        /// <returns></returns>
        public async Task<Step> GetNextStep()
        {
            var newRequest = new StepRequest
            {
                CompatibleDefinitions = RegisteredTemplates.Select(t => t.TemplateId()).ToArray()
            };

            var result = await _client.PostAsync(_client.BaseAddress + "/Steps/next", new StringContent(JsonConvert.SerializeObject(newRequest), Encoding.UTF8, "application/json"));

            //Get content
            var content = await result.Content.ReadAsStringAsync();

            if (content == "null")
            {
                return null;
            }

            //Read the content as a string
            Step step = JsonConvert.DeserializeObject<Step>(content);

            return step;
        }

        public async Task<Step> ProcessStep(Step step)
        {
            try
            {
                if(ValidateStep(step))
                {
                    return await HandleStep(step);
                }
                else
                {
                    Logger.LogError("Unknown error while validating " + step.Id);
                    return null;
                }
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message);
                return null;
            }
        }


        public bool ValidateStep(Step step)
        {
            var foundStepTemplatesCount = RegisteredTemplates.Where(rt => rt.TemplateId() == step.TemplateId).Count();

            if (foundStepTemplatesCount == 0)
            {
                throw new StepTemplateNotFoundException("No step templates for step template " + step.TemplateId);
            }
            else if (foundStepTemplatesCount == 1)
            {
                return true;
            }
            else
            {
                throw new StepTemplateDuplicateFoundException("Found duplicate step templates for step template " + step.TemplateId);
            }
        }

        /// <summary>
        /// This will return the output of a step
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        public abstract Task<Step> HandleStep(Step step);
    }
}
