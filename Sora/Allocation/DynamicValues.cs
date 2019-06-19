using System.Collections.Generic;

namespace Sora.Allocation
{
    public interface IDynamicValues
    {
        object this[object key] { get; set; }
        void Set<T>(object Key, T value);
        T Get<T>(object Key);
    }

    public class DynamicValues : IDynamicValues
    {
        private readonly Dictionary<object, object> _customValues = new Dictionary<object, object>();

        public object this[object key]
        {
            get => !_customValues.TryGetValue(key, out object val) ? default : val;
            set => _customValues[key] = value;
        }

        public void Set<T>(object Key, T value)
        {
            _customValues[Key] = value;
        }

        public T Get<T>(object Key)
        {
            if (!_customValues.TryGetValue(Key, out object val))
                return default;
            return (T) val;
        }
    }
}