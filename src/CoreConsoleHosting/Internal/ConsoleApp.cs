using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Builder;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

namespace CoreConsoleHosting.Hosting.Internal
{
    internal class ConsoleApp : IConsoleApp
    {
        public static ManualResetEvent Done = new ManualResetEvent(false);


        private static readonly string DeprecatedServerUrlsKey = "server.urls";

        private readonly IServiceCollection _applicationServiceCollection;
        private readonly IServiceProvider _hostingServiceProvider;
        private readonly WebHostOptions _options;
        private readonly IConfiguration _config;
        private readonly AggregateException _hostingStartupErrors;
        private readonly IConsoleStartup _startup;
        private ILogger<IConsoleApp> _logger;

        private bool _stopped;

        private ExceptionDispatchInfo _applicationServicesException;

        
        public ConsoleApp(
           IServiceCollection appServices,
           IServiceProvider hostingServiceProvider,
           IConfiguration config,
           WebHostOptions options,
           AggregateException hostingStartupErrors)
        {
            
            _config = config;
            _hostingStartupErrors = hostingStartupErrors;
            _options = options;
            _applicationServiceCollection = appServices;
            _hostingServiceProvider = hostingServiceProvider;
            _startup = _hostingServiceProvider.GetService<IConsoleStartup>();
        }

        public void Initialize()
        {
            
        }

        

        public void Run()
        {
            Console.CancelKeyPress += (sender, eventArgs) =>
            {
                eventArgs.Cancel = true;
                Done.Set();
            };
            Done.WaitOne();
        }

      
        public void Dispose()
        {
            
        }

    }
}
