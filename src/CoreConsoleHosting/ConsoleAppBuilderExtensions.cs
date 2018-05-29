using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.Extensions.DependencyInjection;
using CoreConsoleHosting.Hosting;
using System;
using System.Reflection;

namespace CoreConsoleHosting
{
    public static class ConsoleAppBuilderExtensions
    {
        public static IConsoleAppBuilder UseStartup<TStartup>(this IConsoleAppBuilder hostBuilder) 
            where TStartup : IConsoleStartup, new()
        {
           
            Type startupType = typeof(TStartup);
            var startupAssemblyName = startupType.GetTypeInfo().Assembly.GetName().Name;
            
            return hostBuilder
                .UseSetting(WebHostDefaults.ApplicationKey, startupAssemblyName)
                .ConfigureServices((services) =>
                {
                    hostBuilder.Startup = new TStartup();
                    hostBuilder.Startup.Environment = hostBuilder.Environment;
                    hostBuilder.Startup.ConfigureServices(services);
                    services.AddSingleton(typeof(IConsoleStartup), sp =>
                    {
                        hostBuilder.Startup.Configure(sp);
                        return hostBuilder.Startup;
                    });
                });
        }
    }
}
