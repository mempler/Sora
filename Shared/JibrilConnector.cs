using System;
using Shared.Helpers;
using StackExchange.Redis;

namespace Shared
{
    public enum JibrilConnectorEvents
    {
        SubmitScore,
        IsRelaxing,
        SendMessage
    }

    public interface JibrilConnectorEventArgs
    {
        string Sanitize();
    }

    public class SubmitScore : JibrilConnectorEventArgs
    {
        public int UserId;
        public bool Relaxing;
        
        public string Sanitize()
        {
            return $"{UserId}|{Relaxing}";
        }
    }

    public class IsRelaxingArgs : JibrilConnectorEventArgs
    {
        public int UserId;
        public bool Relaxing;

        public string Sanitize()
        {
            return $"{UserId}|{Relaxing}";
        }
    }

    public class SendMessageArgs : JibrilConnectorEventArgs
    {
        public int UserId;
        public string Channel;
        public string Message;

        public string Sanitize()
        {
            return $"{UserId}|{Channel}|{Message}";
        }
    }
    
    public class JibrilConnector
    {
        private readonly ConnectionMultiplexer _redis;
        public JibrilConnector(IConfig config)
        {
            _redis = ConnectionMultiplexer.Connect(config.Redis.Hostname);
        }

        public void TriggerEvent(JibrilConnectorEvents eventType, JibrilConnectorEventArgs args)
        {
            Random random = new Random();
            _redis.GetDatabase()
                  .HashSet("jibril:connector:events", random.Next(int.MaxValue / 2), eventType + "," + args.Sanitize());
        }
    }
}