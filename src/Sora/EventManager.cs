#region LICENSE

/*
    olSora - A Modular Bancho written in C#
    Copyright (C) 2019 Robin A. P.

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License as
    published by the Free Software Foundation, either version 3 of the
    License, or (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU Affero General Public License for more details.

    You should have received a copy of the GNU Affero General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
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

        private readonly Dictionary<object, List<Tuple<EventType, List<MethodInfo>>>> _events =
            new Dictionary<object, List<Tuple<EventType, List<MethodInfo>>>>();

        public EventManager(IEnumerable<Assembly> asma) => _asma = asma;

        public IServiceProvider Provider { get; private set; }

        /// <summary>
        /// Set Active Service Provider
        /// </summary>
        /// <param name="provider"></param>
        public void SetProvider(IServiceProvider provider)
        {
            Provider = provider;
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
            foreach (var evcls in _events)
            foreach (var (evTypes, mInfos) in evcls.Value)
            {
                if (evTypes != etype)
                    continue;

                foreach (var mInfo in mInfos)
                {
                    var o = new List<object>();
                    if (args != null || mInfo.GetParameters().Length > 0)
                        o.Add(args);
                    var r = mInfo.Invoke(evcls.Key, o.ToArray());
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

                var methods = AttributeUtility.GetTFromMethod<EventAttribute>(t);

                var tArgs = (from cInfo in t.GetConstructors()
                             from pInfo in cInfo.GetParameters()
                             select Provider.GetService(pInfo.ParameterType)).ToArray();

                if (tArgs.Any(x => x == null))
                    throw new Exception("Could not find Dependency, are you sure you registered the Dependency?");

                var cls = Activator.CreateInstance(t, tArgs);

                foreach (var mInfo in methods)
                {
                    var eventTy = mInfo.GetCustomAttributes<EventAttribute>().First().Type;
                    if (_events.All(x => x.Key.GetType() != t))
                        _events.Add(cls, new List<Tuple<EventType, List<MethodInfo>>>());
                    else
                        cls = _events.First(x => x.Key.GetType() == t).Key;

                    if (_events[cls].All(x => x.Item1 != eventTy))
                        _events[cls].Add(new Tuple<EventType, List<MethodInfo>>(eventTy, new List<MethodInfo> {mInfo}));
                    else
                        _events[cls].First(x => x.Item1 == eventTy).Item2.Add(mInfo);
                }
            }
        }
    }
}
