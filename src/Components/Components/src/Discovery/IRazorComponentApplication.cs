// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;
using Microsoft.AspNetCore.Components.Discovery;

namespace Microsoft.AspNetCore.Components;

/// <summary>
/// When applied to a component it signals that it represents the top level
/// component in an application.
/// </summary>
// Note, this lives here because of the router. If we split the router into its
// own assembly we can get rid of this.
// Note that the intention is for this interface to be code-generated for the
// application during build.
// However, we want to provide default implementations so that we can add more
// features in the future without breaking existing implementations by
// providing a default implementation for each member that we include in this interface.
// We leverage interfaces and static members because they allow us to completely avoid
// reflection by means of doing a "double dispatch" based on the type.
// We use a generic argument because it allows us to get "context" statically that we can
// use for reflection purposes when we provide a default implementation.
public interface IRazorComponentApplication<TComponent>
    where TComponent : IRazorComponentApplication<TComponent>
{
    public static virtual Dictionary<string, List<PageComponentMetadata>> GetPageMetadata()
    {
        var componentsAssemblyName = typeof(IComponent).Assembly.GetName();
        // We use the generic argument TComponent to determine the entry point using
        // reflection in the default implementation.
        var result = new Dictionary<string, List<PageComponentMetadata>>();

        var assembly = typeof(TComponent).Assembly!;
        var pages = ScanForPages(assembly);
        result[assembly.GetName().Name] = pages;
        var references = assembly.GetReferencedAssemblies();
        for (var i = 0; i < references.Length; i++)
        {
            var candidate = Assembly.Load(references[i]);
            if (ReferencesComponentsAssembly(candidate, componentsAssemblyName))
            {
                result[candidate.GetName().Name] = ScanForPages(candidate);
            }
        }

        return result;
    }

    private static List<PageComponentMetadata> ScanForPages(Assembly assembly)
    {
        var result = new List<PageComponentMetadata>();
        var types = assembly.GetExportedTypes();
        for (int i = 0; i < types.Length; i++)
        {
            var type = types[i];
            if (!type.IsAssignableTo(typeof(IComponent)))
            {
                continue;
            }

            var route = type.GetCustomAttribute<RouteAttribute>();
            if (route != null)
            {
                result.Add(new PageComponentMetadata(route.Template, type));
            }
        }

        return result;
    }

    private static bool ReferencesComponentsAssembly(Assembly candidate, AssemblyName components)
    {
        var references = candidate.GetReferencedAssemblies();
        for (var i = 0; i < references.Length; i++)
        {
            var reference = references[i];
            if (reference.Equals(components))
            {
                return true;
            }
        }

        return false;
    }
}
