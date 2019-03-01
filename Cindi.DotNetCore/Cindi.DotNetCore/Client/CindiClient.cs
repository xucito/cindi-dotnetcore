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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input">Returns task Id</param>
        /// <returns></returns>
        public async Task<int> PostNewSequence(SequenceInput input)
        {
            using (HttpClient client = new HttpClient())
            {

                client.BaseAddress = new Uri(_url);

                var response = await client.PostAsync("/api/sequences", new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, "application/json"));

                var contents = await response.Content.ReadAsStringAsync();
                
                if(response.IsSuccessStatusCode)
                {
                    return int.Parse(contents);
                }
                else
                {
                    throw new Exception("Error sending sequence request, returned with error " + response.StatusCode);
                }
            }
        }
    }
}
