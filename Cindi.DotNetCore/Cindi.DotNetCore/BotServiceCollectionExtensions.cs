using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cindi.DotNetCore.BotExtensions
{
    public static class ServicesConfiguration
    {
        /// <summary>
        /// Add abstracted service with default options
        /// </summary>
        /// <typeparam name="TProcessor"></typeparam>
        /// <param name="services"></param>
        /// <param name="configureOptions"></param>
        public static void AddWorkerBot<THandler>(this IServiceCollection services, Action<WorkerBotHandlerOptions> configureOptions)
        where THandler : WorkerBotHandler<WorkerBotHandlerOptions>
            {
                if (services == null)
                {
                    throw new ArgumentNullException(nameof(services));
                }

                if (configureOptions == null)
                {
                    throw new ArgumentNullException(nameof(configureOptions));
                }

                services.Configure(configureOptions);

                services.AddSingleton<THandler>();
            }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TOptions"></typeparam>
        /// <typeparam name="TProcessor"></typeparam>
        /// <param name="services"></param>
        /// <param name="configureOptions"></param>
        public static void AddWorkerBot<TOptions, THandler> (this IServiceCollection services, Action<WorkerBotHandlerOptions> configureOptions)
            where TOptions: WorkerBotHandlerOptions
            where THandler : WorkerBotHandler<TOptions>
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configureOptions == null)
            {
                throw new ArgumentNullException(nameof(configureOptions));
            }

            services.Configure(configureOptions);

            services.AddSingleton<THandler>();
        }
    }
}
