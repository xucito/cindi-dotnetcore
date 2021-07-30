using Cindi.Domain.Entities.Steps;
using Cindi.Domain.Entities.StepTemplates;
using Cindi.Domain.Entities.WorkflowsTemplates;
using Cindi.Domain.Utilities;
using Cindi.Domain.ValueObjects;
using Cindi.DotNetCore.BotExtensions.Exceptions;
using Cindi.DotNetCore.BotExtensions.Models;
using Cindi.DotNetCore.BotExtensions.Requests;
using Cindi.DotNetCore.BotExtensions.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Cindi.DotNetCore.BotExtensions.Client
{
    public partial class BotClient : IBotClient
    {
        private string _url;
        private double _nonce;
        private string _botId;
        public RSAEncodedKeyPair keyPair;
        public int maxAttempts = 10;
        HttpClientHandler clientHandler
        {
            get
            {
                var newHandler = new HttpClientHandler();
                newHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                return newHandler;
            }
        }

        public BotClient(string botName, string url)
        {
            _url = url;
            _nonce = 0;
            keyPair = SecurityUtility.GenerateRSAKeyPair(2048);
            var result = RegisterBot(botName, keyPair.PublicKey).GetAwaiter().GetResult();
            _botId = result.IdKey;
        }

        public BotClient(BotClientOptions options)
        {
            _url = options.Url;
            _nonce = 0;
            keyPair = SecurityUtility.GenerateRSAKeyPair(2048);
            var result = RegisterBot(options.Name, keyPair.PublicKey).GetAwaiter().GetResult();
            _botId = result.IdKey;
        }

        private void AuthorizeClientWithBotId(HttpClient client)
        {
            _nonce++;
            var nonceKey = SecurityUtility.RsaEncryptWithPrivate("" + _nonce, keyPair.PrivateKey);
            client.DefaultRequestHeaders.Add("BotKey", _botId);
            client.DefaultRequestHeaders.Add("Nonce", nonceKey);
        }

        private async Task<JObject> SendGetRequest(string resourcePath, bool authorize)
        {
            var attempt = 0;
            Exception lastException = new Exception();
            while (attempt < maxAttempts)
            {
                try
                {
                    using (HttpClient client = new HttpClient(clientHandler))
                    {
                        client.BaseAddress = new Uri(_url);

                        if (authorize)
                        {
                            AuthorizeClientWithBotId(client);
                        }

                        var response = await client.GetAsync(resourcePath);

                        var content = await response.Content.ReadAsStringAsync();

                        if (response.IsSuccessStatusCode)
                        {

                            if (content == null)
                            {
                                return null;
                            }
                            return JObject.Parse(content).Value<JObject>("result");
                        }
                        else
                        {
                            throw new HttpRequestException("Server responded with " + response.StatusCode + ": " + content);
                        }
                    }
                }
                catch (Exception e)
                {
                    BackOff(attempt, "GET " + resourcePath, e, maxAttempts);
                    lastException = e;
                }
                attempt++;
            }
            throw lastException;
        }

        private async Task<JObject> SendPostRequest(string resourcePath, object payload, bool authorize)
        {
            var attempt = 0;
            Exception lastException = new Exception();
            while (attempt < maxAttempts)
            {
                try
                {
                    using (HttpClient client = new HttpClient(clientHandler))
                    {
                        client.BaseAddress = new Uri(_url);

                        if (authorize)
                        {
                            AuthorizeClientWithBotId(client);
                        }

                        var response = await client.PostAsync(resourcePath, new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json"));

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();

                            return JObject.Parse(content);
                        }
                        else
                        {
                            throw new Exception("Post request failed with status code " + response.StatusCode + " and message "  + (await response.Content.ReadAsStringAsync()));
                        }
                    }
                }
                catch (Exception e)
                {
                    BackOff(attempt, "POST" +
                        "" +
                        " " + resourcePath, e, maxAttempts);
                    lastException = e;
                }
                attempt++;
            }

            throw lastException;
        }

        private async Task<JObject> SendPutRequest(string resourcePath, object payload, bool authorize)
        {
            var attempt = 0;
            Exception lastException = new Exception();
            while (attempt < maxAttempts)
            {
                try
                {
                    using (HttpClient client = new HttpClient(clientHandler))
                    {
                        client.BaseAddress = new Uri(_url);

                        if (authorize)
                        {
                            AuthorizeClientWithBotId(client);
                        }

                        var response = await client.PutAsync(resourcePath, new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json"));

                        if (response.IsSuccessStatusCode)
                        {
                            var content = await response.Content.ReadAsStringAsync();
                            return JObject.Parse(content);
                        }
                        else if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                        {
                            throw new BadRequestException(await response.Content.ReadAsStringAsync());
                        }
                        else
                        {
                            throw new RegistrationFailureException();
                        }
                    }
                }
                catch (Exception e)
                {
                    attempt++;
                    BackOff(attempt, "PUT " + resourcePath, e, maxAttempts);
                    lastException = e;
                }
            }

            throw lastException;
        }

        public async Task<NewBotKeyResult> RegisterBot(string name, string rsaPublicKey)
        {
            return (await SendPostRequest("/api/bot-keys", new
            {
                botKeyName = name,
                publicEncryptionKey = rsaPublicKey
            }, false)).Value<JObject>("result").ToObject<NewBotKeyResult>();
        }

        public void SetIdToken(string botId)
        {
            _botId = botId;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input">Returns task Id</param>
        /// <returns></returns>
        public async Task<string> PostNewWorkflow(WorkflowInput input, string idToken)
        {
            return (await SendPostRequest("/api/Workflows", input, true)).Value<string>("objectRefId");
        }

        public async Task<string> PostNewWorkflowTemplate(WorkflowTemplate WorkflowTemplate, string idToken)
        {
            return (await SendPostRequest("/api/Workflow-templates", WorkflowTemplate, true)).Value<string>("objectRefId");
        }

        public async Task<string> PostNewStep(StepInput stepInput, string idToken)
        {
            return (await SendPostRequest("/api/steps", stepInput, true)).Value<string>("objectRefId");
        }

        public async Task<string> AddStepLog(Guid stepId, string log, string idToken)
        {
            return (await SendPostRequest("/api/steps/" + stepId.ToString() + "/logs", new
            {
                Log = log
            }, true)).Value<string>("id");
        }

        public async Task<string> PostStepTemplate(NewStepTemplateRequest stepTemplate, string idToken)
        {
            return (await SendPostRequest("/api/step-templates", stepTemplate, true)).Value<string>("objectRefId");
        }

        public async Task<string> CompleteStep(UpdateStepRequest request, string idToken)
        {
            return (await SendPutRequest("/api/steps/" + request.Id, request, true)).Value<string>("objectRefId");
        }

        public async Task<NextStep> GetNextStep(StepRequest request, string idToken)
        {
            var stepRequestResult = (await SendPostRequest("/api/steps/assignment-requests", request, true));

            if (stepRequestResult == null)
            {
                return null;

            }
            NextStep result = new NextStep();
            result.Step = stepRequestResult["result"].ToObject<Step>();
            result.EncryptionKey = stepRequestResult["encryptionKey"].ToObject<string>();
            return result;
        }

        public async void BackOff(int attempt, string command, Exception e, int maxAttempts)
        {
            var backoffTime = CalculateBackoffTime(attempt);

            if (attempt < maxAttempts)
            {
                Console.WriteLine("Failed to send " + command + " with message " + e.Message + ", will sleep for " + backoffTime + " second and try again, attempt " + attempt);
                await Task.Delay(backoffTime);
            }
            else
            {
                Console.WriteLine("Failed to send " + command + " with message " + e.Message + ".");
            }
        }

        public static int CalculateBackoffTime(int attempt)
        {
            return attempt * 5000;
        }
    }
}
