using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;

using Kaoiji.enums;
using Kaoiji.helpers;
using Kaoiji.objects;

namespace Kaoiji.handler
{
    public abstract class BaseHandler
    {
        public abstract void Run(Presence presence, object data, HttpListenerResponse writer);
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
        private static Dictionary<HandlerTypes, List<BaseHandler>> hndls = new Dictionary<HandlerTypes, List<BaseHandler>>();
        public static void init()
        {
            foreach (BaseHandler x in Reflector.GetEnumerableOfType<BaseHandler>())
            {
                TypeInfo typeInfo = x.GetType().GetTypeInfo();
                var attrs = typeInfo.GetCustomAttributes();
                foreach (RegisterHandlerAttribute attr in attrs)
                {
                    if (!hndls.ContainsKey(attr.Kind))
                        hndls[attr.Kind] = new List<BaseHandler>();
                    hndls[attr.Kind].Add((BaseHandler)Activator.CreateInstance(x.GetType()));
                }
            }
        }
        public static List<BaseHandler> GetHandlers(HandlerTypes type)
        {
            if (hndls.ContainsKey(type))
                return hndls[type];
            else
                return null;
        }
        public static void RunHandlers(List<BaseHandler> h, Presence p, object data, HttpListenerResponse writer)
        {
            foreach (BaseHandler hndl in h)
                hndl.Run(p, data, writer);
        }
    }
}
