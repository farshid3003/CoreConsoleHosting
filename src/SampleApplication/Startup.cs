using CoreConsoleHosting.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SampleApplication.Business;
using System;
using System.Collections.Generic;
using System.Text;

namespace SampleApplication
{
    /// <summary>
    /// Console Startup
    /// </summary>
    public class Startup : IConsoleStartup
    {
        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; set; }
        public IHostingEnvironment Environment { get; set; }

        /// <summary>
        /// Register your service here
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            string sampleConfig = Configuration["SampleConfig"];
            string genericConfig = Configuration["GenericConfig"];

            services.AddSingleton<IBiz, Biz>();
        }

        /// <summary>
        /// Configuration of services
        /// </summary>
        /// <param name="serviceProvider"></param>
        public void Configure(IServiceProvider serviceProvider)
        {
            //serviceProvider.UserSomething();

            ILoggerFactory loggerFactory = serviceProvider.GetService<ILoggerFactory>();
            loggerFactory.AddConsole();


            IBiz biz = serviceProvider.GetService<IBiz>();
            biz.Run();
        }
    }
}
