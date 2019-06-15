#region LICENSE
/*
    Sora - A Modular Bancho written in C#
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
using Sora.Attributes;
using Sora.Enums;
using Sora.Helpers;
using Sora.Interfaces;
using Sora.Utilities;

namespace Sora
{
public class EventManager
    {
        private readonly IEnumerable<Assembly> _asma;

        private readonly Dictionary<object, List<Tuple<EventType, List<MethodInfo>>>> _events =
            new Dictionary<object, List<Tuple<EventType, List<MethodInfo>>>>();
        
        public IServiceProvider Provider { get; private set; }

        public void SetProvider(IServiceProvider provider)
        {
            Provider = provider;
        }
        
        public EventManager(IEnumerable<Assembly> asma)
        {
            _asma = asma;
        }

        public void RegisterEvents()
        {
            foreach (Assembly asm in _asma)
                LoadAssembly(asm);
        }
        
        public async Task RunEvent(EventType etype, IEventArgs args = null)
        {
            foreach (KeyValuePair<object, List<Tuple<EventType, List<MethodInfo>>>> evcls in _events)
            {
                foreach ((EventType evTypes, List<MethodInfo> mInfos) in evcls.Value)
                {
                    if (evTypes != etype) continue;

                    foreach (MethodInfo mInfo in mInfos)
                    {
                        List<object> o = new List<object>();
                        if (args != null || mInfo.GetParameters().Length > 0)
                            o.Add(args);
                        object r = mInfo.Invoke(evcls.Key, o.ToArray());
                        if (r == null) continue;
                        
                        if (r.GetType().IsGenericType)
                            await (Task) r;
                    }
                }
            }
        }

        public void LoadAssembly(Assembly asm)
        {
            IEnumerable<Type> eat = AttributeUtility
                .GetTFromClass<EventClassAttribute>(asm);
            
            foreach (Type t in eat)
            {
                if (!t.IsClass)
                    throw new Exception("Event " + t + " Failed because " + t + " Is not a class!");
                    
                IEnumerable<MethodInfo> methods = AttributeUtility.GetTFromMethod<EventAttribute>(t);

                object[] tArgs = (from cInfo in t.GetConstructors()
                                  from pInfo in cInfo.GetParameters()
                                  select Provider.GetService(pInfo.ParameterType)).ToArray();

                if (tArgs.Any(x => x == null))
                    throw new Exception("Could not find Dependency, are you sure you registered the Dependency?");

                object cls = Activator.CreateInstance(t, tArgs);
                    
                foreach (MethodInfo mInfo in methods)
                {
                    EventType eventTy = mInfo.GetCustomAttributes<EventAttribute>().First().Type;
                    if (_events.All(x => x.Key.GetType() != t))
                        _events.Add(cls, new List<Tuple<EventType, List<MethodInfo>>>());
                    else
                        cls = _events.First(x => x.Key.GetType() == t).Key;
                        
                    if (_events[cls].All(x => x.Item1 != eventTy))
                        _events[cls].Add(new Tuple<EventType, List<MethodInfo>>(eventTy, new List<MethodInfo>() { mInfo }));
                    else
                        _events[cls].First(x => x.Item1 == eventTy).Item2.Add(mInfo);
                }
            }
        }
    }
}