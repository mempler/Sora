using System;
using JetBrains.Annotations;
using Sora.Enums;

namespace Sora.Attributes
{
    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Method)]
    public class EventAttribute : Attribute
    {
        internal readonly EventType Type;

        public EventAttribute(EventType type) => Type = type;
    }
}
