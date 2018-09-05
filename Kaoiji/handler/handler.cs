using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

using Kaoiji.enums;
using Kaoiji.helpers;
using Kaoiji.objects;

namespace Kaoiji.handler
{
    public interface IHandler
    {
        void Run(Presence presence, object data);
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class RegisterHandlerAttribute : Attribute
    {
        public HandlerTypes Kind;
        public RegisterHandlerAttribute(HandlerTypes t)
        {
            Kind = t;
        }
    }

    public class Handlers
    {
        private static Dictionary<HandlerTypes, List<IHandler>> hndls = new Dictionary<HandlerTypes, List<IHandler>>();

        public static void Init()
        {
            foreach (IHandler x in Reflector.GetEnumerableOfType<IHandler>())
            {
                TypeInfo typeInfo = x.GetType().GetTypeInfo();
                var attrs = typeInfo.GetCustomAttributes();
                foreach (RegisterHandlerAttribute attr in attrs)
                {
                    if (!hndls.ContainsKey(attr.Kind))
                        hndls[attr.Kind] = new List<IHandler>();
                    hndls[attr.Kind].Add((IHandler)Activator.CreateInstance(x.GetType()));
                }
            }
        }

        public static List<IHandler> GetHandlers(HandlerTypes type)
        {
            if (hndls.ContainsKey(type))
                return hndls[type];
            else
                return new List<IHandler>();
        }

        public static void RunHandlers(List<IHandler> h, Presence p, object data, HttpListenerResponse writer)
        {
            foreach (IHandler hndl in h)
                hndl.Run(p, data);
        }
    }
}
