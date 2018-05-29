using Microsoft.Extensions.Configuration;
using System;
using Microsoft.AspNetCore.Hosting;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting.Internal;

namespace CoreConsoleHosting
{
    public static class ConsoleApp
    {
        public static IConsoleAppBuilder CreateDefaultBuilder() =>
          CreateDefaultBuilder(args: null);

        public static IConsoleAppBuilder CreateDefaultBuilder(string[] args)
        {
            var builder = new ConsoleAppBuilder();


            if (args != null)
            {
                builder.UseConfiguration(new ConfigurationBuilder().AddCommandLine(args).Build());
            }

            builder.ConfigureAppConfiguration((config, configBuilder) =>
                {
                    var env = config.HostingEnvironment;

                    configBuilder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                          .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true);
                    configBuilder.AddEnvironmentVariables();
                    if (args != null)
                    {
                        configBuilder.AddCommandLine(args);
                    }
                });
            return builder;
        }

    }

   
}
