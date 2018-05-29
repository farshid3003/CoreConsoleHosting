using System;

namespace CoreConsoleHosting
{
    public interface IConsoleApp : IDisposable
    {
        void Initialize();
        void Run();
    }
}
