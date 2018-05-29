using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Builder;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using CoreConsoleHosting.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.ExceptionServices;

namespace CoreConsoleHosting
{
    public interface IConsoleAppBuilder
    {
        IConsoleStartup Startup { get; set; }

        IConsoleApp Build();
        string GetSetting(string key);
        IConsoleAppBuilder UseSetting(string key, string value);
        IConsoleAppBuilder UseConfiguration(IConfiguration configuration);
        IConsoleAppBuilder ConfigureServices(Action<IServiceCollection> configureServices);
        IConsoleAppBuilder ConfigureServices(Action<IConfiguration, IServiceCollection> configureServices);
        IConsoleAppBuilder ConfigureAppConfiguration(Action<WebHostBuilderContext, IConfigurationBuilder> configureDelegate);
        HostingEnvironment Environment { get; }
        IConfiguration Configuration { get; }
    }

    public class ConsoleAppBuilder : IConsoleAppBuilder
    {
        public IConfiguration Configuration { get; }
        private bool _consoleAppBuilt;
        private readonly List<Action<IConfiguration, IServiceCollection>> _configureServicesDelegates;
        private List<Action<WebHostBuilderContext, IConfigurationBuilder>> _configureAppConfigurationBuilderDelegates;
        private WebHostBuilderContext _context;
        public HostingEnvironment Environment { get; private set; }
        private WebHostOptions _options;

        public IConsoleStartup Startup { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ConsoleAppBuilder()
        {
            Environment = new HostingEnvironment();
            _configureServicesDelegates = new List<Action<IConfiguration, IServiceCollection>>();
            _configureAppConfigurationBuilderDelegates = new List<Action<WebHostBuilderContext, IConfigurationBuilder>>();

            Configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables(prefix: "ASPNETCORE_")
                .Build();

            if (string.IsNullOrEmpty(GetSetting(WebHostDefaults.EnvironmentKey)))
            {
                // Try adding legacy environment keys, never remove these.
                UseSetting(WebHostDefaults.EnvironmentKey, System.Environment.GetEnvironmentVariable("Hosting:Environment")
                    ?? System.Environment.GetEnvironmentVariable("ASPNET_ENV"));
            }

            _context = new WebHostBuilderContext
            {
                Configuration = Configuration
            };
        }

        /// <summary>
        /// Get the setting value from the configuration.
        /// </summary>
        /// <param name="key">The key of the setting to look up.</param>
        /// <returns>The value the setting currently contains.</returns>
        public string GetSetting(string key)
        {
            return Configuration[key];
        }

        /// <summary>
        /// Add or replace a setting in the configuration.
        /// </summary>
        /// <param name="key">The key of the setting to add or replace.</param>
        /// <param name="value">The value of the setting to add or replace.</param>
        /// <returns>The <see cref="IWebHostBuilder"/>.</returns>
        public IConsoleAppBuilder UseSetting(string key, string value)
        {
            Configuration[key] = value;
            return this;
        }


        /// <summary>
        /// Builds the required services and an <see cref="IWebHost"/> which hosts a web application.
        /// </summary>
        public IConsoleApp Build()
        {
            if (_consoleAppBuilt)
            {
                throw new InvalidOperationException("Error on build console");
            }
            _consoleAppBuilt = true;

            var hostingServices = BuildCommonServices(out var hostingStartupErrors);
            var hostingServiceProvider = GetProviderFromFactory(hostingServices);

            IConsoleApp host = new Hosting.Internal.ConsoleApp(
                hostingServices,
                hostingServiceProvider,
                Configuration,
                _options,
                hostingStartupErrors);
            try
            {
                host.Initialize();

                return host;
            }
            catch (Exception exc)
            {
                // Dispose the host if there's a failure to initialize, this should clean up
                // will dispose services that were constructed until the exception was thrown
                host.Dispose();
                throw;
            }



            IServiceProvider GetProviderFromFactory(IServiceCollection collection)
            {
                var provider = collection.BuildServiceProvider();
                var factory = provider.GetService<IServiceProviderFactory<IServiceCollection>>();

                if (factory != null)
                {
                    using (provider)
                    {
                        return factory.CreateServiceProvider(factory.CreateBuilder(collection));
                    }
                }

                return provider;
            }
        }

        private IServiceCollection BuildCommonServices(out AggregateException hostingStartupErrors)
        {
            hostingStartupErrors = null;

            _options = new WebHostOptions(Configuration);

            var services = new ServiceCollection();


            var contentRootPath = ResolveContentRootPath(_options.ContentRootPath, AppContext.BaseDirectory);

            Environment.Initialize(Assembly.GetEntryAssembly()?.GetName().Name, contentRootPath, _options);

            _context.HostingEnvironment = Environment;

            services.AddSingleton<IHostingEnvironment>(Environment);


            var builder = new ConfigurationBuilder()
               .SetBasePath(Environment.ContentRootPath)
               .AddConfiguration(Configuration);

            foreach (var configureAppConfiguration in _configureAppConfigurationBuilderDelegates)
            {
                configureAppConfiguration(_context, builder);
            }

            var configuration = builder.Build();
            services.AddSingleton<IConfiguration>(configuration);
            _context.Configuration = configuration;

            services.AddTransient<IServiceProviderFactory<IServiceCollection>, DefaultServiceProviderFactory>();

            services.AddOptions();

            services.AddLogging();


            foreach (var configureServices in _configureServicesDelegates)
            {
                configureServices(Configuration, services);
            }

            return services;
        }

        /// <summary>
        /// Adds a delegate for configuring additional services for the host or web application. This may be called
        /// multiple times.
        /// </summary>
        /// <param name="configureServices">A delegate for configuring the <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IWebHostBuilder"/>.</returns>
        public IConsoleAppBuilder ConfigureServices(Action<IServiceCollection> configureServices)
        {
            if (configureServices == null)
            {
                throw new ArgumentNullException(nameof(configureServices));
            }

            return ConfigureServices((_, services) => configureServices(services));
        }

        /// <summary>
        /// Adds a delegate for configuring additional services for the host or web application. This may be called
        /// multiple times.
        /// </summary>
        /// <param name="configureServices">A delegate for configuring the <see cref="IServiceCollection"/>.</param>
        /// <returns>The <see cref="IWebHostBuilder"/>.</returns>
        public IConsoleAppBuilder ConfigureServices(Action<IConfiguration, IServiceCollection> configureServices)
        {
            if (configureServices == null)
            {
                throw new ArgumentNullException(nameof(configureServices));
            }

            _configureServicesDelegates.Add(configureServices);
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public IConsoleAppBuilder UseConfiguration(IConfiguration configuration)
        {
            foreach (var setting in configuration.AsEnumerable())
            {
                this.UseSetting(setting.Key, setting.Value);
            }

            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="configureDelegate"></param>
        /// <returns></returns>
        public IConsoleAppBuilder ConfigureAppConfiguration(Action<WebHostBuilderContext, IConfigurationBuilder> configureDelegate)
        {
            if (configureDelegate == null)
            {
                throw new ArgumentNullException(nameof(configureDelegate));
            }

            _configureAppConfigurationBuilderDelegates.Add(configureDelegate);
            return this;
        }


        private string ResolveContentRootPath(string contentRootPath, string basePath)
        {
            if (string.IsNullOrEmpty(contentRootPath))
            {
                return basePath;
            }
            if (Path.IsPathRooted(contentRootPath))
            {
                return contentRootPath;
            }
            return Path.Combine(Path.GetFullPath(basePath), contentRootPath);
        }
    }


}
