using System;
using EventManager.Enums;
using JetBrains.Annotations;

namespace EventManager.Attributes
{
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Method)]
    public class EventAttribute : Attribute
    {
        internal readonly EventType Type;

        public EventAttribute(EventType type) => Type = type;
    }
}