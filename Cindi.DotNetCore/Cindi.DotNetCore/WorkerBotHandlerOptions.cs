using Cindi.Domain.Entities.StepTemplates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cindi.DotNetCore.BotExtensions
{
    public class WorkerBotHandlerOptions
    {
        public string NodeURL { get; set; }
        /// <summary>
        /// In MS how long the bot should sleep each loop
        /// </summary>
        public int SleepTime { get; set; }
        /// <summary>
        /// Step Templates that this bot can process
        /// </summary>
        public List<StepTemplate> StepTemplateLibrary { get; set; }
        public bool AutoStart { get; set; }
        public string Id { get; set; }
        public bool AutoRegister = true;
        public int KeyLength { get; set; } = 1024;
    }
}
