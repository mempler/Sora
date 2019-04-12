using System.Threading;
using EventManager;
using Shared.Helpers;
using Sora.Helpers;
using StackExchange.Redis;

namespace JibrilConnector
{
    public class PluginEntry : Plugin
    {
        private ConnectionMultiplexer Redis;
        private Thread UpdateLoopThread;

        private bool Stop = false;
        
        public override void OnEnable()
        {
            Logger.Info("Startup JibrilConnector");
            
            Redis = ConnectionMultiplexer.Connect("localhost");
            UpdateLoopThread = new Thread(UpdateThread);
            
            Logger.Info("Begin Thread");
        }

        public override void OnDisable()
        {
            Logger.Info("Shutdown JibrilConnector");

            Stop = false;
            UpdateLoopThread.Join();
        }

        private void UpdateThread()
        {
            while (!Stop)
            {
                Redis.GetDatabase().HashGetAll("connector:events:*");
                
                Thread.Sleep(1000);
            }
        }
    }
}