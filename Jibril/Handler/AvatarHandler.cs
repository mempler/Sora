namespace Jibril.Handler
{
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Text;
    using Shared.Enums;
    using Shared.Handlers;

    public class AvatarHandler
    {
        [Handler(HandlerTypes.Initializer)]
        public static void Initialize()
        {
            if (!Directory.Exists("data/avatars"))
                Directory.CreateDirectory("data/avatars");
        }
        
        [Handler(HandlerTypes.SharedAvatars)]
        public void OnAvatarRequest(HttpListenerRequest request, HttpListenerResponse response, Dictionary<string, string> args)
        {
            args.TryGetValue("avatar", out string avi);
            int.TryParse(avi ?? "0", out int avatarId);
            if (avatarId == 0 || !File.Exists($"data/avatars/{avatarId}"))
            {
                if (File.Exists("data/avatars/default"))
                    response.OutputStream.Write(File.ReadAllBytes("data/avatars/default"));
                else
                    response.OutputStream.Write(Encoding.ASCII.GetBytes("Sorry to tell you, but there is no default avatar!\n" +
                                                                        "if you're the owner of this instance please set an default " +
                                                                        "by adding a png/jpg file called default"));
                return;
            }
            
            response.OutputStream.Write(File.ReadAllBytes($"data/avatars/{avatarId}"));
        }
    }
}
