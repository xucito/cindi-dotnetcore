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
using Cindi.DotNetCore.BotExtensions.Requests;
using Cindi.Domain.Entities.Steps;
using Cindi.Domain.Entities.StepTemplates;
using Newtonsoft.Json.Linq;
using Cindi.DotNetCore.BotExtensions.Client;
using System.Security;
using Cindi.Domain.Utilities;
using Cindi.DotNetCore.BotExtensions.ViewModels;

namespace Cindi.DotNetCore.BotExtensions
{
    public abstract class WorkerBotHandler<TOptions> where TOptions : WorkerBotHandlerOptions
    {
        public string nodeUrl { get; }
        private BotClient _client;
        Thread serviceThread;
        private bool started = false;
        private object threadLocker = new Object();
        private int waitTime = 1000;
        private object waitTimeLocker = new Object();
        private bool _hasValidHttpClient = false;
        public ILogger Logger;
        protected UrlEncoder UrlEncoder { get; }
        public TOptions Options { get; }
        public int loopNumber = 0;
        public string Id { get; }
        public string RunTime { get; }
        private string idToken;

        public string DecryptionKey { get; set; }

        public WorkerBotHandler(TOptions options, ILoggerFactory logger, UrlEncoder encoder)
        {
            if (options.NodeURL != null && options.NodeURL != "")
            {
                this.nodeUrl = options.NodeURL;
                _client = new BotClient(new BotClientOptions()
                {
                    Url = this.nodeUrl,
                    Name = options.BotName
                });
                _hasValidHttpClient = true;
            }

            Options = options;

            //RegisterBot().GetAwaiter().GetResult();

            waitTime = options.SleepTime;

            if (options.NodeURL != null && options.NodeURL != "")
            {
                // Register the step template
                foreach (var template in options.StepTemplateLibrary)
                {
                    // Queue all the templates for registration
                    QueueTemplateForRegistration(template);
                }
            }

            if (logger != null)
            {
                Logger = logger.CreateLogger(this.GetType().FullName);
            }

            if (options.Id == null)
            {
                Random rnd = new Random();
                Id = BotUtility.GenerateName(rnd.Next(4, 10)) + '-' + (rnd.Next(1, 100));
            }

            // Create a new Run Time Id
            RunTime = Guid.NewGuid().ToString();

            StartWorking();
        }

        public WorkerBotHandler(IOptionsMonitor<TOptions> options, ILoggerFactory logger, UrlEncoder encoder)
        {
            if (options.CurrentValue.NodeURL != null && options.CurrentValue.NodeURL != "")
            {
                this.nodeUrl = options.CurrentValue.NodeURL;
                _client = new BotClient(new BotClientOptions()
                {
                    Url = this.nodeUrl,
                    Name = options.CurrentValue.BotName
                });
                _hasValidHttpClient = true;
            }

            Options = options.CurrentValue;

            // RegisterBot().GetAwaiter().GetResult();

            waitTime = options.CurrentValue.SleepTime;

            // Register the step template
            foreach (var template in options.CurrentValue.StepTemplateLibrary)
            {
                // Queue all the templates for registration
                QueueTemplateForRegistration(template);
            }

            if (logger != null)
            {
                Logger = logger.CreateLogger(this.GetType().FullName);
            }

            if (options.CurrentValue.Id == null)
            {
                Random rnd = new Random();
                Id = BotUtility.GenerateName(rnd.Next(4, 10)) + '-' + (rnd.Next(1, 100));
            }

            // Create a new Run Time Id
            RunTime = Guid.NewGuid().ToString();

            // Initiate the registration of all templates and run loop if valid
            StartWorking();
        }

        /*public async Task<bool> RegisterBot()
         {
             var keys = Cindi.Domain.Utilities.SecurityUtility.GenerateRSAKeyPair(Options.KeyLength);
             privateSecretEncryptionKey = keys.PrivateKey;
             var encryptedIdToken = await _client.RegisterBot(Options.Id, keys.PublicKey);
             idToken = encryptedIdToken.IdKey;
             publicKey = keys.PublicKey;
             return true;
         }*/


        /// <summary>
        /// List of templates this bot can run against
        /// </summary>
        public List<StepTemplate> RegisteredTemplates = new List<StepTemplate>();

        public async void StartWorking()
        {
            if (Options.AutoRegister)
            {
                await RegisterAllTemplatesAsync();
            }
            if (Options.AutoStart)
            {
                if (_hasValidHttpClient)
                {
                    Run();
                }
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
                    Logger.LogWarning("Could not register template " + template.Id + " as there is no valid httpClient.");
                }
            }
            return true;
        }

        public void QueueTemplateForRegistration(StepTemplate stepTemplate)
        {
            var foundTemplateCount = RegisteredTemplates.Where(rt => rt.ReferenceId == stepTemplate.ReferenceId).Count();

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
            var result = await _client.PostStepTemplate(new NewStepTemplateRequest()
            {
                Name = stepTemplate.Name,
                Version = stepTemplate.Version,
                AllowDynamicInputs = stepTemplate.AllowDynamicInputs,
                InputDefinitions = stepTemplate.InputDefinitions,
                OutputDefinitions = stepTemplate.OutputDefinitions
            }, idToken);

            if (result != null)
            {
                Logger.LogInformation("Successfully registered template " + stepTemplate.ReferenceId);
                return true;
            }
            else
            {
                throw new Exception("Error adding template for template " + stepTemplate.ReferenceId);
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
            Logger.LogDebug("Starting loops.");
            var stopWatch = new Stopwatch();
            while (started)
            {
                Logger.LogDebug("Starting new Thread");
                Step nextStep = null;
                string encryptionKey = "";
                try
                {
                    var response = await GetNextStep();
                    nextStep = response.Step;
                    encryptionKey = response.EncryptionKey;
                }
                catch (Exception e)
                {
                    Logger.LogWarning("Error getting next step, will sleep and try again. " + e.Message);
                }
                stopWatch.Reset();
                stopWatch.Start();

                UpdateStepRequest stepResult = new UpdateStepRequest();

                string newEncryptionKey = SecurityUtility.RandomString(32, false);

                if (nextStep != null)
                {
                    Logger.LogInformation("Processing step " + nextStep.Id);
                    stepResult.Id = nextStep.Id;
                    string decryptedEncryptionKey = encryptionKey != null && encryptionKey != "" ? SecurityUtility.RsaDecryptWithPrivate(encryptionKey, _client.keyPair.PrivateKey): "";
                    nextStep.Inputs = DynamicDataUtility.DecryptDynamicData(RegisteredTemplates.Where(rt => rt.ReferenceId == nextStep.StepTemplateId).First().InputDefinitions, nextStep.Inputs, Domain.Enums.EncryptionProtocol
                        .AES256, decryptedEncryptionKey, false);

                    var template = RegisteredTemplates.Where(rt => rt.ReferenceId == nextStep.StepTemplateId).First();
                    try
                    {
                        stepResult = await ProcessStep(nextStep);
                        stepResult.Outputs = DynamicDataUtility.EncryptDynamicData(template.OutputDefinitions, stepResult.Outputs, Domain.Enums.EncryptionProtocol.AES256, newEncryptionKey);
                        stepResult.EncryptionKey = SecurityUtility.RsaEncryptWithPrivate(newEncryptionKey, _client.keyPair.PrivateKey);
                    }
                    catch (Exception e)
                    {
                        await SendStepLog(nextStep.Id, "Encountered unexcepted error " + e.Message + Environment.NewLine + e.StackTrace);
                        stepResult.Status = StepStatuses.Error;
                        stepResult.Log = "Encountered uncaught error at " + e.Message + ".";
                    }

                    int count = 0;
                    string success = "";
                    try
                    {
                        success = await _client.CompleteStep(stepResult, idToken);
                        Logger.LogInformation("Completed step " + stepResult.Id + " with status " + stepResult.Status);
                    }
                    catch (Exception e)
                    {
                        Logger.LogError("Failed to save step "+ stepResult.Id + " with status " + stepResult.Status + " in Cindi with exception " + e.Message + ".");
                        Thread.Sleep(1000);
                    }
                }
                else
                {
                    Logger.LogDebug("No step found");
                }

                loopNumber++;
                stopWatch.Stop();
                Logger.LogDebug("Completed Service Loop " + loopNumber + " took approximately " + stopWatch.ElapsedMilliseconds + "ms");

                //Only sleep if no step was picked up
                if (nextStep == null)
                {
                    lock (waitTimeLocker)
                    {
                        Logger.LogDebug("Sleeping for " + waitTime + "ms");
                        Thread.Sleep(waitTime);
                    }
                }
            }
        }

        /// <summary>
        /// Get the next step based on defintions acceptable
        /// </summary>
        /// <returns></returns>
        public async Task<NextStep> GetNextStep()
        {
            var newRequest = new StepRequest
            {
                StepTemplateIds = RegisteredTemplates.Select(t => t.ReferenceId).ToArray()
            };
            return await _client.GetNextStep(newRequest, idToken);
        }

        public async Task<UpdateStepRequest> ProcessStep(Step step)
        {
            try
            {
                if (ValidateStep(step))
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
                throw e;
            }
        }


        public bool ValidateStep(Step step)
        {
            var foundStepTemplatesCount = RegisteredTemplates.Where(rt => rt.ReferenceId == step.StepTemplateId).Count();

            if (foundStepTemplatesCount == 0)
            {
                throw new StepTemplateNotFoundException("No step templates for step template " + step.StepTemplateId);
            }
            else if (foundStepTemplatesCount == 1)
            {
                return true;
            }
            else
            {
                throw new StepTemplateDuplicateFoundException("Found duplicate step templates for step template " + step.StepTemplateId);
            }
        }



        public async Task<string> SendStepLog(Guid stepId, string logMessage)
        {
            return await _client.AddStepLog(stepId, logMessage, idToken);
        }

        /// <summary>
        /// This will return the output of a step
        /// </summary>
        /// <param name="step"></param>
        /// <returns></returns>
        public abstract Task<UpdateStepRequest> HandleStep(Step step);
    }
}
