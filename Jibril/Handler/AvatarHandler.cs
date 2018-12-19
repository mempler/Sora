using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using Shared.Enums;
using Shared.Handlers;
using Shared.Helpers;

namespace Jibril.Handler
{
    public class AvatarHandler
    {
        [Handler(HandlerTypes.Initializer)]
        public static void Initialize()
        {
            if (!Directory.Exists("data/avatars"))
                Directory.CreateDirectory("data/avatars");
        }

        [Handler(HandlerTypes.SharedAvatars)]
        public void OnAvatarRequest(HttpListenerRequest request, HttpListenerResponse response,
                                    Dictionary<string, string> args)
        {
            args.TryGetValue("avatar", out string avi);
            int.TryParse(avi ?? "0", out int avatarId);
            
            byte[] cachedData = Cache.GetCachedData($"jibril:avatars:{avatarId.ToString()}");
            if (cachedData != null && cachedData.Length > 0)
            {
                response.OutputStream.Write(cachedData);
                return;
            }

            byte[] data;
            
            if (avatarId == 0 || !File.Exists($"data/avatars/{avatarId.ToString()}"))
            {
                if (File.Exists("data/avatars/default"))
                    data = File.ReadAllBytes("data/avatars/default");
                else
                {
                    response.OutputStream.Write(Encoding.ASCII.GetBytes(
                                                    "Sorry to tell you, but there is no default avatar!\n" +
                                                    "if you're the owner of this instance please set an default " +
                                                    "by adding a png/jpg file called default"));
                    return;
                }
            } else
                data = File.ReadAllBytes($"data/avatars/{avatarId.ToString()}");

            Cache.CacheData($"jibril:avatars:{avatarId.ToString()}", data);
            
            response.OutputStream.Write(data);
        }
    }
}