using System.IO;
using System.Text;
using EventManager.Attributes;
using EventManager.Enums;
using Jibril.EventArgs;
using Shared.Helpers;

namespace Jibril.Events
{
    [EventClass]
    public class OnAvatarsEvent
    {
        private readonly Cache _cache;

        public OnAvatarsEvent(Cache cache)
        {
            _cache = cache;
            
            if (!Directory.Exists("data/avatars"))
                Directory.CreateDirectory("data/avatars");
        }
        
        [Event(EventType.SharedAvatars)]
        public void OnAvatars(SharedEventArgs args)
        {
            args.args.TryGetValue("avatar", out string avi);
            int.TryParse(avi ?? "0", out int avatarId);

            byte[] cachedData = _cache.GetCachedData($"jibril:avatars:{avatarId.ToString()}");
            if (cachedData != null && cachedData.Length > 0)
            {
                args.res.OutputStream.Write(cachedData);
                return;
            }

            byte[] data;

            if (avatarId == 0 || !File.Exists($"data/avatars/{avatarId.ToString()}"))
            {
                if (File.Exists("data/avatars/default"))
                    data = File.ReadAllBytes("data/avatars/default");
                else
                {
                    args.res.OutputStream.Write(Encoding.ASCII.GetBytes(
                                                    "Sorry to tell you, but there is no default avatar!\n" +
                                                    "if you're the owner of this instance please set an default " +
                                                    "by adding a png/jpg file called default"));
                    return;
                }
            }
            else
                data = File.ReadAllBytes($"data/avatars/{avatarId.ToString()}");

            _cache.CacheData($"jibril:avatars:{avatarId.ToString()}", data);

            args.res.OutputStream.Write(data);
        }
    }
}