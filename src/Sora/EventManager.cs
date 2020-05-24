using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Sora.Enums;
using Sora.Framework.Utilities;
using Sora.Attributes;


namespace Sora
{
    public interface IEventArgs
    {
    }
    
    public class EventManager
    {
        private readonly IEnumerable<Assembly> _asma;

        private Dictionary<EventType, List<Type>> _eventClasses = new Dictionary<EventType, List<Type>>();
        
        public EventManager(IEnumerable<Assembly> asma) => _asma = asma;

        public IServiceProvider Provider { get; private set; }

        /// <summary>
        /// Set Active Service Provider
        /// </summary>
        /// <param name="provider"></param>
        public void SetProvider(IApplicationBuilder app)
        {
            Provider = app.ApplicationServices;
        }

        /// <summary>
        /// Register Events from all assemblies passed by Constructor
        /// </summary>
        public void RegisterEvents()
        {
            foreach (var asm in _asma)
                LoadAssembly(asm);
        }

        /// <summary>
        /// Run a Specified Event based on EventType
        /// </summary>
        /// <param name="etype">an EventType to determine which event should be called</param>
        /// <param name="args">This will throw an Error if you choose the wrong EventArgs!</param>
        /// <returns>Awaiter</returns>
        public async Task RunEvent(EventType etype, IEventArgs args = null)
        {
            if (!_eventClasses.ContainsKey(etype))
            {
                Logger.Warn($"Event {etype} isn't implemented anywhere!");
                return;
            }

            foreach (var type in _eventClasses[etype])
            {
                var methods = AttributeUtility.GetTFromMethod<EventAttribute>(type);

                var tArgs = (from cInfo in type.GetConstructors()
                             from pInfo in cInfo.GetParameters()
                             select Provider.GetService(pInfo.ParameterType)).ToArray();

                for (var i = 0; i < tArgs.Length; i++)
                {
                    if (tArgs[i].GetType() == typeof(IServiceProvider))
                        tArgs[i] = Provider;
                }
                
                if (tArgs.Any(x => x == null))
                    throw new Exception("Could not find Dependency, are you sure you registered the Dependency?");

                var cls = Activator.CreateInstance(type, tArgs);
                
                foreach (var method in methods)
                {
                    var o = new List<object>();
                    if (args != null || method.GetParameters().Length > 0)
                        o.Add(args);
                    var r = method.Invoke(cls, o.ToArray());
                    if (r == null)
                        continue;

                    if (r.GetType().IsGenericType)
                        await (Task) r;
                }
            }
        }

        /// <summary>
        /// LoadAssembly into EventManager for Plugins and Self!
        /// </summary>
        /// <param name="asm">Assembly of the given Plugin / Application</param>
        /// <exception cref="Exception">Given Dependencies hasn't been found</exception>
        public void LoadAssembly(Assembly asm)
        {
            var eat = AttributeUtility
                .GetTFromClass<EventClassAttribute>(asm);

            foreach (var t in eat)
            {
                if (!t.IsClass)
                    throw new Exception("Event " + t + " Failed because " + t + " Is not a class!");

                var eventTypes = AttributeUtility.GetTFromMethod<EventAttribute>(t)
                                                 .Select(s => s.GetCustomAttribute<EventAttribute>()?.Type)
                                                 .Where(s => s.HasValue)
                                                 .Select(s => s.Value);

                foreach (var eType in eventTypes)
                {
                    if (_eventClasses.TryGetValue(eType, out var types)) // TODO: finish
                    {
                        if (!types.Contains(t))
                            types.Add(t);   
                    }
                    else
                    {
                        _eventClasses.Add(eType, new List<Type>{ t });
                    }
                }
            }
        }
    }
}
