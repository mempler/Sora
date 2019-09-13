using System.Collections.Generic;

namespace Sora.Framework.Allocation
{
    public interface IDynamicValues
    {
        object this[object key] { get; set; }
        void Set<T>(object Key, T value);
        bool TryGet<T>(object Key, out T obj);
    }

    public class DynamicValues : IDynamicValues
    {
        private readonly Dictionary<object, object> _customValues = new Dictionary<object, object>();

        public object this[object key]
        {
            get => !_customValues.TryGetValue(key, out var val) ? default : val;
            set => _customValues[key] = value;
        }

        public void Set<T>(object Key, T value)
        {
            _customValues[Key] = value;
        }
        
        public bool TryGet<T>(object key, out T obj)
        {
            var o = _customValues.TryGetValue(key, out var dObj);
            obj = (T) dObj;
            return o;
        }
    }
}
