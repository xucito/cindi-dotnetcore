using Cindi.Domain.Entities.JournalEntries;
using Cindi.Domain.Entities.Steps;
using Cindi.Domain.ValueObjects;
using Cindi.DotNetCore.BotExtensions.Requests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cindi.DotNetCore.BotExtensions
{
    [Route("api/[Controller]")]
    public abstract class BotController<THandler> : BotController<WorkerBotHandlerOptions, THandler>
        where THandler : WorkerBotHandler<WorkerBotHandlerOptions>
    {
        public BotController(THandler handler, ILoggerFactory logger): base( handler, handler.Options, logger)
        {}
    }

    [Route("api/[Controller]")]
    public abstract class BotController<TOptions, THandler> : Controller
        where TOptions : WorkerBotHandlerOptions
        where THandler : WorkerBotHandler<TOptions>
    {
        private readonly THandler Handler;
        private readonly ILogger Logger;
        private readonly TOptions Options;

        public BotController(THandler handler, TOptions options, ILoggerFactory logger)
        {
            Handler = handler;
            Logger = logger.CreateLogger(this.GetType().FullName);
            Options = options;
        }

        [HttpGet("step-templates")]
        public IActionResult GetAvailableStepTemplates()
        {
            return Ok(Handler.RegisteredTemplates);
        }

        [HttpPost("step")]
        public async Task<IActionResult> ProcessStep([FromBody] OnDemandStepRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newStepId = Guid.NewGuid();
            return Ok(await Handler.ProcessStep(new Step()
            {
                Id = newStepId,
                StepTemplateId = request.StepTemplateId,
                CreatedOn = DateTime.UtcNow,
                Journal = new Domain.Entities.JournalEntries.Journal(new List<JournalEntry>() {
                    new JournalEntry()
                    {
                        SubjectId = newStepId,
                        ChainId = 0,
                        Entity = JournalEntityTypes.Step,
                        CreatedOn = DateTime.UtcNow,
                        Updates = new List<Update>()
                        {
                            new Update()
                            {
                                FieldName = "status",
                                Value = StepStatuses.Unassigned,
                                Type = UpdateType.Override
                            }
                        }
                    } }),
                Inputs = request.Inputs
            }));
        }
    }
}
