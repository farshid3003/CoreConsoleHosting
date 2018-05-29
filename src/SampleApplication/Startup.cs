using CoreConsoleHosting.Hosting;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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
            //services.AddSingleton<IService, Service>();
        }

        /// <summary>
        /// Configuration of services
        /// </summary>
        /// <param name="serviceProvider"></param>
        public void Configure(IServiceProvider serviceProvider)
        {
            //serviceProvider.UserSomething();
        }
    }
}
