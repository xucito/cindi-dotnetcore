﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cindi.DotNetCore.BotExtensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SampleBot.Services;
using Swashbuckle.AspNetCore.Swagger;

namespace SampleBot
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddWorkerBot<SampleWorkerBot>(o =>
            {
                o.NodeURL = Configuration.GetValue<string>("cindiurl");
                o.BotName = Configuration.GetValue<string>("botName");
                o.SleepTime = 100;
                o.StepTemplateLibrary = new List<Cindi.Domain.Entities.StepTemplates.StepTemplate>() {
                    Library.SecretStepTemplate,
                    Library.StepTemplate
                };
                o.AutoStart = true;
            });
            services.AddSingleton<SampleService>();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSwaggerGen(c =>
            {
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, SampleWorkerBot bot
         /* , SampleService sampleService*/
            )
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), 
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

        }
    }
}
