using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Shared.Enums;
using Shared.Handlers;
using Shared.Models;
using File = System.IO.File;

namespace Jibril.Handler
{
    public class GetReplayHandler
    {
        [Handler(HandlerTypes.SharedGetReplay)]
        public void OnReplayRequest(HttpListenerRequest request, HttpListenerResponse response,
                                      Dictionary<string, string> args)
        {
            NameValueCollection query = HttpUtility.ParseQueryString(request.Url.Query);
            Users user = Users.GetUser(Users.GetUserId(query.Get("u")));
            if (user == null || !user.IsPassword(query.Get("h")))
            {
                response.OutputStream.Write(Encoding.ASCII.GetBytes("error: pass"));
                return;
            }

            try
            {
                int replayId = int.Parse(query.Get("c"));
                Scores score = Scores.GetScore(replayId);
                using(FileStream s = File.OpenRead("data/replays/" + score.ReplayMd5 + ".rbin"))
                    s.CopyTo(response.OutputStream);
            }
            catch
            {
                // Ignored
            }
        }
    }
}