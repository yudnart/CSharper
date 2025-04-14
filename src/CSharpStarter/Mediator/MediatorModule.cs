using CSharpStarter.Utilities;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace CSharpStarter.Mediator;

/// <summary>
/// Provides extension methods for configuring the mediator service in the dependency injection container.
/// </summary>
public static class MediatorModule
{
    /// <summary>
    /// Adds the mediator service to the specified <see cref="IServiceCollection"/> with a scoped lifetime.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the mediator service to.</param>
    /// <returns>The same <see cref="IServiceCollection"/> instance for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is null.</exception>
    public static IServiceCollection AddMediator(this IServiceCollection services)
    {
        services.ThrowIfNull(nameof(services));
        services.AddScoped<IMediator, SimpleMediator>();
        return services;
    }
}
