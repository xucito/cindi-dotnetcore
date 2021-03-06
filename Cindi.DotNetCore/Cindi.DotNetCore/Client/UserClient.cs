﻿using Cindi.Domain.Entities.Steps;
using Cindi.Domain.Entities.Workflows;
using Cindi.Domain.Entities.WorkflowsTemplates;
using Cindi.Domain.Enums;
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
    public class UserClient
    {
        private string _url;
        private string _username;
        private string _password;
        HttpClientHandler clientHandler
        {
            get
            {
                var newHandler = new HttpClientHandler();
                newHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                return newHandler;
            }
        }

        public UserClient(string url, string username, string password)
        {
            _url = url;
            _username = username;
            _password = password;
        }

        public UserClient(CindiClientOptions options)
        {
            _url = options.Url;
        }

        private void AuthorizeClientWithUser(HttpClient client, string username = null, string password = null)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(
                    System.Text.ASCIIEncoding.ASCII.GetBytes(
                string.Format("{0}:{1}", username != null ? username : _username, password != null ? password : _password))));
        }

        public async Task<Workflow> PostNewWorkflow(WorkflowInput input, string username = null, string password = null)
        {
            using (HttpClient client = new HttpClient(clientHandler))
            {
                client.BaseAddress = new Uri(_url);
                AuthorizeClientWithUser(client, username, password);

                var response = await client.PostAsync("/api/Workflows", new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, "application/json"));

                var contents = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JObject.Parse(contents)["result"].ToObject<Workflow>();
                }
                else
                {
                    throw new Exception("Error sending Workflow request, returned with error " + response.StatusCode + " with message " + contents);
                }
            }
        }

        public async Task<bool> PostNewWorkflowTemplate(WorkflowTemplate WorkflowTemplate, string username = null, string password = null)
        {
            using (HttpClient client = new HttpClient(clientHandler))
            {
                client.BaseAddress = new Uri(_url);
                AuthorizeClientWithUser(client, username, password);

                var body = JsonConvert.SerializeObject(WorkflowTemplate);
                var response = await client.PostAsync("/api/Workflow-templates", new StringContent(body, Encoding.UTF8, "application/json"));

                var contents = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    throw new Exception("Error sending Workflow template request, returned with error " + response.StatusCode + "with message " + contents);
                }
            }
        }

        public async Task<Step> PostNewStep(StepInput stepInput, string username = null, string password = null)
        {
            using (HttpClient client = new HttpClient(clientHandler))
            {
                client.BaseAddress = new Uri(_url);
                AuthorizeClientWithUser(client, username, password);

                var response = await client.PostAsync("/api/steps", new StringContent(JsonConvert.SerializeObject(stepInput), Encoding.UTF8, "application/json"));

                var contents = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JObject.Parse(contents)["result"].ToObject<Step>();
                }
                else
                {
                    throw new Exception("Error sending Workflow template request, returned with error " + response.StatusCode);
                }
            }
        }

        public async Task<bool> PostStepTemplate(NewStepTemplateRequest stepTemplate, string username = null, string password = null)
        {
            using (HttpClient client = new HttpClient(clientHandler))
            {
                client.BaseAddress = new Uri(_url);

                AuthorizeClientWithUser(client, username, password);

                var response = await client.PostAsync("/api/step-templates", new StringContent(JsonConvert.SerializeObject(stepTemplate), Encoding.UTF8, "application/json"));

                var contents = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    throw new Exception("Error sending Workflow template request, returned with error " + response.StatusCode);
                }
            }
        }

        public async Task<Step> GetStep(Guid stepId, string username = null, string password = null)
        {
            using (HttpClient client = new HttpClient(clientHandler))
            {
                client.BaseAddress = new Uri(_url);

                AuthorizeClientWithUser(client, username, password);

                var response = await client.GetAsync("/api/steps/" + stepId);

                var contents = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JObject.Parse(contents)["result"].ToObject<Step>();
                }
                else
                {
                    throw new Exception("Error sending Workflow template request, returned with error " + response.StatusCode);
                }
            }
        }

        public async Task<List<Step>> GetSteps(string username = null, string password = null)
        {
            using (HttpClient client = new HttpClient(clientHandler))
            {
                client.BaseAddress = new Uri(_url);

                AuthorizeClientWithUser(client, username, password);

                var response = await client.GetAsync("/api/steps/");

                var contents = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var array = JObject.Parse(contents).Value<JArray>("result");
                    return array.ToObject<List<Step>>();
                }
                else
                {
                    throw new Exception("Error sending Workflow template request, returned with error " + response.StatusCode);
                }
            }
        }

        public async Task<Workflow> GetWorkflow(Guid WorkflowId, string username = null, string password = null)
        {
            using (HttpClient client = new HttpClient(clientHandler))
            {
                client.BaseAddress = new Uri(_url);

                AuthorizeClientWithUser(client, username, password);

                var response = await client.GetAsync("/api/Workflows/" + WorkflowId);

                var contents = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return JObject.Parse(contents)["result"].ToObject<Workflow>();
                }
                else
                {
                    throw new Exception("Error sending Workflow template request, returned with error " + response.StatusCode);
                }
            }
        }

        public async Task<List<Workflow>> GetWorkflows(string username = null, string password = null)
        {
            using (HttpClient client = new HttpClient(clientHandler))
            {
                client.BaseAddress = new Uri(_url);

                AuthorizeClientWithUser(client, username, password);

                var response = await client.GetAsync("/api/Workflows?size=1000");

                var contents = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var array = JObject.Parse(contents).Value<JArray>("result");
                    return array.ToObject<List<Workflow>>();
                }
                else
                {
                    throw new Exception("Error sending Workflow template request, returned with error " + response.StatusCode);
                }
            }
        }

        public async Task<List<Step>> GetWorkflowSteps(Guid workflowId, string username = null, string password = null)
        {
            using (HttpClient client = new HttpClient(clientHandler))
            {
                client.BaseAddress = new Uri(_url);

                AuthorizeClientWithUser(client, username, password);

                var response = await client.GetAsync("/api/Workflows/" + workflowId.ToString() + "/steps");

                var contents = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var array = JObject.Parse(contents).Value<JArray>("result");
                    return array.ToObject<List<Step>>();
                }
                else
                {
                    throw new Exception("Error sending Workflow template request, returned with error " + response.StatusCode);
                }
            }
        }

        public async Task<bool> PostGlobalValue(string name, string type, string description, object value, string username = null, string password = null)
        {
            if (!InputDataTypes.IsValidDataType(type))
            {
                throw new Exception("Global Value " + name + " has invalid type " + type);
            }

            using (HttpClient client = new HttpClient(clientHandler))
            {
                client.BaseAddress = new Uri(_url);

                AuthorizeClientWithUser(client, username, password);

                var response = await client.PostAsync("/api/global-values", new StringContent(JsonConvert.SerializeObject(new
                {
                    name = name,
                    type = type,
                    description = description,
                    value = value
                }), Encoding.UTF8, "application/json"));

                var contents = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    throw new Exception("Error sending Global Value template request, returned with error " + response.StatusCode);
                }
            }
        }

        public async Task<bool> PostExecutionSchedule(string name, string description, string executionTemplate, string[] schedule, bool runImmediately = false, string username = null, string password = null)
        {
            using (HttpClient client = new HttpClient(clientHandler))
            {
                client.BaseAddress = new Uri(_url);

                AuthorizeClientWithUser(client, username, password);

                var response = await client.PostAsync("/api/execution-schedules?runImmediately=" + runImmediately, new StringContent(JsonConvert.SerializeObject(new
                {
                    name = name,
                    description = description,
                    executionTemplateName = executionTemplate,
                    schedule = schedule
                }), Encoding.UTF8, "application/json"));

                var contents = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    throw new Exception("Error sending execution schedule request, returned with error " + response.StatusCode);
                }
            }
        }
    }
}
