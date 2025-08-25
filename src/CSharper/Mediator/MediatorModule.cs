using CSharper.Extensions;
using CSharper.RequestContext;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace CSharper.Mediator;

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
    public static IServiceCollection AddSimpleMediator(this IServiceCollection services)
    {
        services.ThrowIfNull(nameof(services));
        services.AddScoped<IMediator, SimpleMediator>();
        return services;
    }

    /// <summary>
    /// Adds the <see cref="LoggingBehavior"/> as a scoped <see cref="IBehavior"/> to the DI container.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is null.</exception>
    /// <remarks>
    /// Requires an <see cref="IRequestContext"/> implementation to be registered beforehand using
    /// <c>services.AddScoped&lt;IRequestContext, YourRequestContext&gt;()</c>. If not registered,
    /// the DI container will throw an <see cref="InvalidOperationException"/> when resolving
    /// <see cref="LoggingBehavior"/>. Use the overloads with a type or factory to register
    /// <see cref="IRequestContext"/> as part of this method.
    /// </remarks>
    public static IServiceCollection AddLoggingBehavior(this IServiceCollection services)
    {
        services.ThrowIfNull(nameof(services));
        services.AddScoped<IBehavior, LoggingBehavior>();
        return services;
    }

    /// <summary>
    /// Adds the <see cref="LoggingBehavior"/> as a scoped <see cref="IBehavior"/> and the specified
    /// <see cref="IRequestContext"/> implementation to the DI container.
    /// </summary>
    /// <typeparam name="TContext">The type implementing <see cref="IRequestContext"/> to provide request-specific metadata.</typeparam>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> is null.</exception>
    public static IServiceCollection AddLoggingBehavior<TContext>(this IServiceCollection services)
        where TContext : class, IRequestContext
    {
        services.ThrowIfNull(nameof(services));
        services.TryAddScoped<IRequestContext, TContext>();
        services.AddScoped<IBehavior, LoggingBehavior>();
        return services;
    }

    /// <summary>
    /// Adds the <see cref="LoggingBehavior"/> as a scoped <see cref="IBehavior"/> and an
    /// <see cref="IRequestContext"/> implementation via a factory to the DI container.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services to.</param>
    /// <param name="requestContextFactory">A factory delegate to create the <see cref="IRequestContext"/> instance.</param>
    /// <returns>The <see cref="IServiceCollection"/> for chaining.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="services"/> or <paramref name="requestContextFactory"/> is null.</exception>
    public static IServiceCollection AddLoggingBehavior(
        this IServiceCollection services, 
        Func<IServiceProvider, IRequestContext> requestContextFactory)
    {
        services.ThrowIfNull(nameof(services));
        requestContextFactory.ThrowIfNull(nameof(requestContextFactory));
        services.TryAddScoped(requestContextFactory);
        services.AddScoped<IBehavior, LoggingBehavior>();
        return services;
    }
}
