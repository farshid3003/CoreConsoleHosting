using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace CoreConsoleHosting.Hosting
{
    public interface IConsoleStartup
    {
        IConfiguration Configuration { get; set; }
        IHostingEnvironment Environment { get; set; }
        void ConfigureServices(IServiceCollection services);
        void Configure(IServiceProvider serviceProvider);
    }
}
