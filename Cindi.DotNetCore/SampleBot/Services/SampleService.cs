using Cindi.DotNetCore.BotExtensions.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SampleBot.Services
{
    public class SampleService
    {
        CindiClient _client;
        List<Thread> _testThreads = new List<Thread>();

        public SampleService()
        {
            _client = new CindiClient("http://localhost:5021");

            _testThreads.Add(new Thread(() =>
            {
                _client.PostNewSequence
            }));

        }
    }
}
