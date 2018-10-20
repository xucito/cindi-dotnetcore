using Cindi.DotNetCore.BotExtensions.Models;
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

                var result = await client.PostAsync("/sequences", new StringContent(JsonConvert.SerializeObject(input), Encoding.UTF8, "application/json"));

                var test = result.Content;

                return 1;
            }
        }
    }
}
