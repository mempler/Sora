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