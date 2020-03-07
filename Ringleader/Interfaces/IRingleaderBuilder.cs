using Microsoft.Extensions.DependencyInjection;

namespace Ringleader
{
    public interface IRingleaderBuilder
    {
        IServiceCollection Services { get; }
        bool UsingCustomRegistry { get; set; }
        bool IsClientFactorySet { get; set; }
        bool IsHandlerFactorySet { get; set; }
        void Build();
    }
}
