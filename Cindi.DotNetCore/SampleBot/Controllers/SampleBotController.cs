using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cindi.DotNetCore.BotExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SampleBot.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SampleBot.Controllers
{
    [Route("api/[controller]")]
    public class SampleBotController : BotController<SampleWorkerBot>
    {
        public SampleBotController(SampleWorkerBot bot, ILoggerFactory logger): base (bot, logger)
        {

        }
    }
}
