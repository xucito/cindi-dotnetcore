using Cindi.Domain.Entities.SequencesTemplates;
using Cindi.DotNetCore.BotExtensions.Requests;
using Cindi.DotNetCore.BotExtensions.ViewModels;
using Newtonsoft.Json;
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

        public UserClient(string url, string username, string password)
        {
            _url = url;
        }

        public UserClient(CindiClientOptions options)
        {
            _url = options.Url;
        }

        private void AuthorizeClientWithUser(HttpClient client, string username, string password)
        {
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(
                    System.Text.ASCIIEncoding.ASCII.GetBytes(
                string.Format("{0}:{1}", username, password))));
        }

        public async Task<string> PostNewSequence(SequenceInput input, string username, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_url);
                AuthorizeClientWithUser(client, username, password);

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

        public async Task<bool> PostNewSequenceTemplate(SequenceTemplate sequenceTemplate, string username, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_url);
                AuthorizeClientWithUser(client, username, password);

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

        public async Task<bool> PostNewStep(StepInput stepInput, string username, string password)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_url);
                AuthorizeClientWithUser(client, username, password);

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

        public async Task<bool> PostStepTemplate(NewStepTemplateRequest stepTemplate, string username, string password)
        {
            using (HttpClient client = new HttpClient())
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
                    throw new Exception("Error sending sequence template request, returned with error " + response.StatusCode);
                }
            }
        }
    }
}
