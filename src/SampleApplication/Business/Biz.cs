using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace SampleApplication.Business
{
    public class Biz : IBiz
    {
        private readonly ILogger<Biz> _logger;

        public Biz(ILogger<Biz> logger)
        {
            _logger = logger;

            _logger.LogInformation("I got the logger!");
        }

        public void Run()
        {
            _logger.LogInformation("Some Body Call Run!");
        }
    }
}
