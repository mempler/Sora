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

namespace EventManager.Utilities
{
    public static class AttributeUtility
    {
        public static IEnumerable<Type> GetTFromClass<T>(Assembly asm)
            where T : Attribute
        {
            IEnumerable<Type> types =
                from t in asm.GetTypes()
                where t.GetCustomAttributes<T>(false)
                       .Any(x => x != null)
                select t;
            return types;
        }

        public static IEnumerable<MethodInfo> GetTFromMethod<T>(Type classType)
            where T : Attribute
        {
            IEnumerable<MethodInfo> methods =
                from m in classType.GetMethods()
                where m.GetCustomAttributes<T>(false)
                       .Any(x => x != null)
                select m;

            return methods;
        }

    }
}