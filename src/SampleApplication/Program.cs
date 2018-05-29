using CoreConsoleHosting;
using System;

namespace SampleApplication
{
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
}
