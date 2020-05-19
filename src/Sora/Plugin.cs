using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;

namespace Sora
{
    public interface IPlugin
    {
        /// <summary>
        /// Enable Plugin
        /// </summary>
        void OnEnable(IApplicationBuilder app);
        
        /// <summary>
        /// Disable Plugin
        /// </summary>
        void OnDisable();
    }

    [MeansImplicitUse]
    public class Plugin : IPlugin
    {
        public virtual void OnEnable(IApplicationBuilder app)
        {
        }

        public virtual void OnDisable()
        {
        }
    }
}
