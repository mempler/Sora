using System;
using JetBrains.Annotations;

namespace Sora.Attributes
{
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Class)]
    public class EventClassAttribute : Attribute
    {
    }
}
