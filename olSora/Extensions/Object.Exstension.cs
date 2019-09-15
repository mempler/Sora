namespace Sora.Extensions
{
    public static class Object_Extension
    {
        public static T As<T>(this object obj)
        {
            if (obj?.GetType() != typeof(T))
                return default;
            
            return (T) obj;
        }
        
    }
}
