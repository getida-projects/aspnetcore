// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Infrastructure;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection;

/// <summary>
/// 
/// </summary>
public static class RazorComponentsServiceCollectionExtensions
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="services"></param>
    /// <returns></returns>
    public static IRazorComponentsBuilder AddRazorComponents(this IServiceCollection services)
    {
        services.TryAddSingleton<RazorComponentsMarkerService>();

        // Endpoints
        services.TryAddSingleton<RazorComponentEndpointDataSourceFactory>();
        services.TryAddSingleton<RazorComponentEndpointFactory>();

        // TODO: Register common services required for server side rendering

        return new DefaultRazorcomponentsBuilder(services);
    }

    private sealed class DefaultRazorcomponentsBuilder : IRazorComponentsBuilder
    {
        public DefaultRazorcomponentsBuilder(IServiceCollection services)
        {
            Services = services;
        }

        public IServiceCollection Services { get; }
    }
}
