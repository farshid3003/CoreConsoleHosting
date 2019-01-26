# CoreConsoleHosting
Dotnet Core Console Appplication hosting. Add Startup like AspNet Core
Using Dotnet Core Dependency Injection Logger and other stuffs in a console application

Install Nuget Package

```
Install-Package CoreConsoleHosting 
```

Add the following code in your program.cs file

```
    class Program
    {
        static void Main(string[] args)
        {
            BuildConsoleApp(args).Run();
        }

        public static IConsoleApp BuildConsoleApp(string[] args) =>
            ConsoleApp.CreateDefaultBuilder(args)
               .UseStartup<Startup>()
               .Build();
    }

```

Add a new Startup.cs file and copy followin content on it. 

```
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
```


