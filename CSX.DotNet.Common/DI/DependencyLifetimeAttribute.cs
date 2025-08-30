using Microsoft.Extensions.DependencyInjection;
using System;

namespace CSX.DotNet.Common.DI;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public class DependencyLifetimeAttribute : Attribute
{
    public ServiceLifetime Lifetime { get; }

    public DependencyLifetimeAttribute(ServiceLifetime lifetime)
        => Lifetime = lifetime;
}