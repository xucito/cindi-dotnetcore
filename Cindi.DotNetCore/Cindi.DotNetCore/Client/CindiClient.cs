using Cindi.DotNetCore.BotExtensions.ViewModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Cindi.DotNetCore.BotExtensions.Client
{
    public class CindiClient
    {
        private string _url;

        public CindiClient(string url)
        {
            _url = url;
        }

        public CindiClient(CindiClientOptions options)
        {
            _url = options.Url;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input">Returns task Id</param>
        /// <returns></returns>
        public async Task<string> PostNewSequence(SequenceInput input)
        {
            using (HttpClient client = new HttpClient())
            {

                client.BaseAddress = new Uri(_url);

                var response = await client.PostAsync("/api/sequences", new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, "application/json"));

                var contents = await response.Content.ReadAsStringAsync();
                
                if(response.IsSuccessStatusCode)
                {
                    return contents;
                }
                else
                {
                    throw new Exception("Error sending sequence request, returned with error " + response.StatusCode + " with message " + contents);
                }
            }
        }

        public async Task<bool> PostNewSequenceTemplate(SequenceTemplate sequenceTemplate)
        {
            using (HttpClient client = new HttpClient())
            {
                client.BaseAddress = new Uri(_url);

                var response = await client.PostAsync("/api/sequencetemplates", new StringContent(JsonConvert.SerializeObject(sequenceTemplate), Encoding.UTF8, "application/json"));

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
