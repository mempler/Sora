using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Sora.Enums;

namespace Sora.Handler
{
    internal static class Handlers
    {
        private static Dictionary<HandlerTypes, List<MethodInfo>> _handlers = new Dictionary<HandlerTypes, List<MethodInfo>>();
        public static void InitHandlers(Assembly ass, bool reload)
        {
            if (reload)
            {
                _handlers = null;
                GC.Collect(); // Collect garbage.
                _handlers = new Dictionary<HandlerTypes, List<MethodInfo>>();
            }
            var methods = ass.GetTypes()
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttributes(typeof(HandlerAttribute), false).Length > 0)
                .ToArray();

            foreach (var handler in methods)
            {
                var type = ((HandlerAttribute[]) handler.GetCustomAttributes(typeof(HandlerAttribute)))[0].Type;
                if (!_handlers.ContainsKey(type))
                    _handlers[type] = new List<MethodInfo>();
                _handlers[type].Add(handler);
            }
        }

        public static void ExecuteHandler(HandlerTypes type, params object[] args)
        {
            if (_handlers == null) return;
            if (!_handlers.ContainsKey(type)) return;
            var handlers = _handlers[type];
            foreach (var h in handlers)
            {
                var handlerClass = Activator.CreateInstance(h.DeclaringType ?? throw new InvalidOperationException());
                h.Invoke(handlerClass, args);
            }
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class HandlerAttribute : Attribute
    {
        public HandlerTypes Type;
        public HandlerAttribute(HandlerTypes t) => Type = t;
    }
}
