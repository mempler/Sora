#region copyright

/*
MIT License

Copyright (c) 2018 Robin A. P.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using JetBrains.Annotations;
using Shared.Enums;
using Shared.Helpers;

namespace Shared.Handlers
{
    public static class Handlers
    {
        private static Dictionary<HandlerTypes, List<MethodInfo>> _handlers =
            new Dictionary<HandlerTypes, List<MethodInfo>>();

        public static void InitHandlers(Assembly ass, bool reload)
        {
            if (reload)
            {
                _handlers = null;
                GC.Collect(); // Collect garbage.
                _handlers = new Dictionary<HandlerTypes, List<MethodInfo>>();
            }

            MethodInfo[] methods = ass.GetTypes()
                                      .SelectMany(t => t.GetMethods())
                                      .Where(m => m.GetCustomAttributes(typeof(HandlerAttribute), false).Length > 0)
                                      .ToArray();

            foreach (MethodInfo handler in methods)
            {
                HandlerTypes type = ((HandlerAttribute[]) handler.GetCustomAttributes(typeof(HandlerAttribute)))[0]
                    .Type;
                if (!_handlers.ContainsKey(type))
                    _handlers[type] = new List<MethodInfo>();
                _handlers[type].Add(handler);
            }
        }

        public static void ExecuteHandler(HandlerTypes type, params object[] args)
        {
            if (_handlers == null) return;
            if (!_handlers.ContainsKey(type)) return;
            List<MethodInfo> handlers = _handlers[type];
            foreach (MethodInfo h in handlers)
            {
                if (h.IsStatic)
                {
                    try { h.Invoke(null, args); }
                    catch (TargetInvocationException tie) { Logger.L.Error(tie.InnerException); }

                    return; // Dont handle it as a non static method. just return.
                }

                object handlerClass =
                    Activator.CreateInstance(h.DeclaringType ?? throw new InvalidOperationException());
                try { h.Invoke(handlerClass, args); }
                catch (TargetInvocationException tie) { Logger.L.Error(tie.InnerException); }
            }
        }
    }

    [MeansImplicitUse]
    [AttributeUsage(AttributeTargets.Method)]
    public class HandlerAttribute : Attribute
    {
        public readonly HandlerTypes Type;

        public HandlerAttribute(HandlerTypes t) { Type = t; }
    }
}