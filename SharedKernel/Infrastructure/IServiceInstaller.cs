using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace SharedKernel.Infrastructure;

public interface IServiceInstaller
{
    public void Install(IServiceCollection services, IConfiguration configuration);
}