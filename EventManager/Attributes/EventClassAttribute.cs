using System;
using JetBrains.Annotations;

namespace EventManager.Attributes
{
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Class)]
    public class EventClassAttribute : Attribute
    {

    }
}