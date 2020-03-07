using System;
using Microsoft.Extensions.DependencyInjection;

namespace Ringleader
{
    public abstract class RingleaderBuilder : IRingleaderBuilder
    {
        public IServiceCollection Services { get; }

        public bool UsingCustomRegistry { get; set; } = false;
        public bool IsClientFactorySet { get; set; } = false;
        public bool IsHandlerFactorySet { get; set; } = false;

        public RingleaderBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }
        public abstract void Build();
    }
}
