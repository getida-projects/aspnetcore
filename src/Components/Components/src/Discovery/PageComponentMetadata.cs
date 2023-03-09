// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Microsoft.AspNetCore.Components.Discovery;

public class PageComponentMetadata
{
    public PageComponentMetadata(string template, Type type)
    {
        Template = template;
        PageType = type;
    }

    /// <summary>
    /// 
    /// </summary>
    public string Template { get; }

    /// <summary>
    /// 
    /// </summary>
    public Type PageType { get; }
}
