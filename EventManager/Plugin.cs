using JetBrains.Annotations;

namespace EventManager
{
    public interface IPlugin
    {
        void OnEnable();
        void OnDisable();
    }

    [MeansImplicitUse]
    public class Plugin : IPlugin
    {
        public virtual void OnEnable()
        {
        }

        public virtual void OnDisable()
        {
        }
    }
}