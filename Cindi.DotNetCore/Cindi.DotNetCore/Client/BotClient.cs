using Cindi.Domain.Entities.SequencesTemplates;
using Cindi.Domain.Entities.Steps;
using Cindi.Domain.Entities.StepTemplates;
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
using System.Threading.Tasks;

namespace Cindi.DotNetCore.BotExtensions.Client
{
    public partial class BotClient
    {
        private string _url;
        private double _nonce;
        private string _botId;
        private string _privateKey;
        private RSAEncodedKeyPair keyPair;

        public BotClient(string url)
        {
            _url = url;
            _nonce = 0;
            keyPair = SecurityUtility.GenerateRSAKeyPair();
            var result = RegisterBot("", keyPair.PublicKey).GetAwaiter().GetResult();
            _botId = result.IdKey;
        }

        public BotClient(BotClientOptions options)
        {
            _url = options.Url;
            _nonce = 0;
            keyPair = SecurityUtility.GenerateRSAKeyPair();
            var result = RegisterBot("", keyPair.PublicKey).GetAwaiter().GetResult();
            _botId = result.IdKey;
        }

        private void AuthorizeClientWithBotId(HttpClient client)
        {
            _nonce++;
            var nonceKey = SecurityUtility.RsaEncryptWithPrivate(""+_nonce, keyPair.PrivateKey);
            client.DefaultRequestHeaders.Add("BotKey", _botId);
            client.DefaultRequestHeaders.Add("Nonce", nonceKey);
        }

        public async Task<NewBotKeyResult> RegisterBot(string name, string rsaPublicKey)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_url);

                var response = await client.PostAsync("/api/bot-keys", new StringContent(JsonConvert.SerializeObject(
                    new
                    {
                        botKeyName = name,
                        publicEncryptionKey = rsaPublicKey
                    }), Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();

                    NewBotKeyResult botKeyResult = JObject.Parse(content)["result"].ToObject<NewBotKeyResult>();
                    return botKeyResult;
                }
                else
                {
                    throw new RegistrationFailureException();
                }
            }
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
        public async Task<string> PostNewSequence(SequenceInput input, string idToken)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_url);
                AuthorizeClientWithBotId(client);

                var response = await client.PostAsync("/api/sequences", new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, "application/json"));

                var contents = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return contents;
                }
                else
                {
                    throw new Exception("Error sending sequence request, returned with error " + response.StatusCode + " with message " + contents);
                }
            }
        }

        public async Task<bool> PostNewSequenceTemplate(SequenceTemplate sequenceTemplate, string idToken)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_url);
                AuthorizeClientWithBotId(client);

                var body = JsonConvert.SerializeObject(sequenceTemplate);
                var response = await client.PostAsync("/api/sequence-templates", new StringContent(body, Encoding.UTF8, "application/json"));

                var contents = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    throw new Exception("Error sending sequence template request, returned with error " + response.StatusCode);
                }
            }
        }

        public async Task<bool> PostNewStep(StepInput stepInput, string idToken)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_url);
                AuthorizeClientWithBotId(client);

                var response = await client.PostAsync("/api/steps", new StringContent(JsonConvert.SerializeObject(stepInput), Encoding.UTF8, "application/json"));

                var contents = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    throw new Exception("Error sending sequence template request, returned with error " + response.StatusCode);
                }
            }
        }

        public async Task<bool> AddStepLog(Guid stepId, string log, string idToken)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_url);
                AuthorizeClientWithBotId(client);

                var response = await client.PostAsync("/api/steps/" + stepId.ToString() + "/logs", new StringContent(JsonConvert.SerializeObject(new
                {
                    Log = log
                }), Encoding.UTF8, "application/json"));

                var contents = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    throw new Exception("Error sending sequence request, returned with error " + response.StatusCode + " with message " + contents);
                }
            }
        }

        public async Task<bool> PostStepTemplate(NewStepTemplateRequest stepTemplate, string idToken)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_url);

                AuthorizeClientWithBotId(client);

                var response = await client.PostAsync("/api/step-templates", new StringContent(JsonConvert.SerializeObject(stepTemplate), Encoding.UTF8, "application/json"));

                var contents = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    throw new Exception("Error sending sequence template request, returned with error " + response.StatusCode);
                }
            }
        }

        public async Task<bool> CompleteStep(UpdateStepRequest request, string idToken)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_url);
                AuthorizeClientWithBotId(client);

                var response = await client.PutAsync("/api/steps/" + request.Id, new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));

                var contents = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    throw new Exception("Error sending sequence template request, returned with error " + response.StatusCode);
                }
            }
        }

        public async Task<Step> GetNextStep(StepRequest request, string idToken)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_url);
                AuthorizeClientWithBotId(client);

                var result = await client.PostAsync("/api/steps/assignment-requests", new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json"));

                var content = await result.Content.ReadAsStringAsync();

                if (content == "null")
                {
                    return null;
                }

                //Read the content as a string
                Step step = JObject.Parse(content)["result"].ToObject<Step>();

                return step;
            }
        }
    }
}
