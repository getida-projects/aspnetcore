// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Microsoft.AspNetCore.Components.Discovery;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;

namespace Microsoft.AspNetCore.Builder;

internal class RazorComponentEndpointFactory
{
    private static readonly HttpMethodMetadata HttpGet = new(new[] { HttpMethods.Get });

    internal void AddEndpoints(
        List<Endpoint> endpoints,
        PageComponentMetadata metadata,
        IReadOnlyList<Action<EndpointBuilder>> conventions)
    {
        // Todo support group conventions, finally conventions, group finally conventions
        var builder = new RouteEndpointBuilder(null, RoutePatternFactory.Parse(metadata.Template), order: 0);
        foreach (var attribute in metadata.PageType.GetCustomAttributes(inherit: true))
        {
            builder.Metadata.Add(attribute);
            builder.Metadata.Add(HttpGet);
        }

        foreach (var convention in conventions)
        {
            convention(builder);
        }

        builder.RequestDelegate = CreateRouteDelegate(metadata.PageType);

        endpoints.Add(builder.Build());
    }

    private RequestDelegate CreateRouteDelegate(Type pageType)
    {
        // TODO: Proper endpoint implementation https://github.com/dotnet/aspnetcore/issues/46988
        return (ctx) =>
        {
            ctx.Response.StatusCode = 200;
            ctx.Response.ContentType = "text/html; charset=utf-8";
            return ctx.Response.WriteAsync($"""
<!DOCTYPE html>
<html lang="en">
  <head>
    <meta charset="UTF-8">
    <meta name="viewport" content="width=device-width, initial-scale=1.0">
    <meta http-equiv="X-UA-Compatible" content="ie=edge">
    <title>{pageType.FullName}</title>
    <link rel="stylesheet" href="style.css">
  </head>
  <body>
	<p>{pageType.FullName}</p>
  </body>
</html>
""");
        };
    }
}
