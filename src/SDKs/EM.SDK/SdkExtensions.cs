using EM.SDK.Departments;
using EM.SDK.Roles;

using Microsoft.Extensions.DependencyInjection;

using Refit;

namespace EM.SDK;

public static class SdkExtensions
{
    /// <summary>
    /// Registers the EM API to the DI Container
    /// </summary>
    /// <param name="services">DI Container</param>
    /// <returns>A reference to the Service Collection for chaining calls</returns>
    public static IServiceCollection AddEmployeeManagement(this IServiceCollection services)
    {
        string url = "https://localhost:7157";

        services.AddRefitClient<IRolesApi>()
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri(url);
            })
            .AddUserAccessTokenHandler();

        services.AddRefitClient<IDepartmentsApi>()
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri(url);
            })
            .AddUserAccessTokenHandler();

        services.AddRefitClient<IDepartmentsApi>()
            .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri(url);
            })
            .AddUserAccessTokenHandler();

        return services;
    }
}
