using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sora.Enums;
using Sora.EventArgs;
using Sora.Objects;
using Sora.Packets.Server;
using Sora.Services;
using Logger = Sora.Helpers.Logger;
using MStreamReader = Sora.Helpers.MStreamReader;
using MStreamWriter = Sora.Helpers.MStreamWriter;
using PacketId = Sora.Enums.PacketId;

namespace Sora.Controllers
{
    [ApiController]
    [Route("/")]
    public class IndexController : Controller
    {
        private readonly ChannelService _cs;
        private readonly EventManager _evmng;
        private readonly PresenceService _ps;

        public IndexController(ChannelService cs,
                               EventManager evmng,
                               PresenceService ps)
        {
            _cs = cs;
            _evmng = evmng;
            _ps = ps;
        } 
        
        [HttpGet]
        public IActionResult IndexGET()
        {
            return Ok();
        }

        [HttpPost]
        //[Produces("application/octet-stream")]
        public async Task<IActionResult> IndexPOST(
                [FromHeader(Name = "osu-token")] string clientToken = null
            )
        {
            try
            {
                Response.Headers["cho-protocol"] = "19";
                Response.Headers["Connection"]   = "keep-alive";
                Response.Headers["Keep-Alive"]   = "timeout=60, max=100";
                Response.Headers["cho-server"]   = "Sora (https://github.com/Mempler/Sora)";

                Response.StatusCode  = 200;

                await using MemoryStream body = new MemoryStream();
                await Request.Body.CopyToAsync(body);
                body.Position = 0;

                await using MStreamWriter mw = MStreamWriter.New();
                using MStreamReader mr = new MStreamReader(body);
                
                Presence pr;
                try
                {
                    if (string.IsNullOrEmpty(clientToken))
                    {
                        pr                               = new Presence(_cs);
                        Response.Headers["cho-token"]    = pr.Token;
                        string ip                        = Response.Headers["X-Forwarded-For"];
                        
                        if (string.IsNullOrEmpty(ip))
                            ip = "127.0.0.1";
                        
                        await _evmng.RunEvent(EventType.BanchoLoginRequest, new BanchoLoginRequestArgs
                        {
                            pr = pr, Reader = mr, Writer = mw, IPAddress = ip
                        });
                        
                        mw.Flush();
                        
                        return await RetOut(mw.BaseStream);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Err(ex);
                    mw.Write(new LoginResponse(LoginResponses.Exception));
                    return await RetOut(mw.BaseStream);
                }

                pr = _ps.GetPresence(Request.Headers["osu-token"]);
                if (pr == null)
                {
                    Logger.Warn("Presence of token%#F94848%", Request.Headers["osu-token"], "%#FFFFFF%hasn't been found!");
                    Response.StatusCode = 403; // Presence is not known, force the client to send login info.
                    return await RetOut(mw.BaseStream);
                }                    
                while (true)
                    try
                    {
                        pr.LastRequest.Restart();
                        if (Request.ContentLength - body.Position < 7)
                            break; // Dont handle any invalid packets! (less then bytelength of 7)

                        PacketId packetId = (PacketId) mr.ReadInt16();
                        mr.ReadBoolean();
                        byte[] packetData = mr.ReadBytes();

                        await using MemoryStream packetDataStream = new MemoryStream(packetData);
                        using MStreamReader packetDataReader = new MStreamReader(packetDataStream);
                        
                        await _evmng.RunEvent(EventType.BanchoPacket, new BanchoPacketArgs
                        {
                            pr = pr, PacketId = packetId, Data = packetDataReader
                        });
                    }
                    catch (Exception ex)
                    {
                        Logger.Err(ex);
                        break;
                    }

                try
                {
                    if (Response.Body.CanWrite)
                        pr.GetOutput()
                          .WriteTo(Response.Body);
                    return await RetOut(mw.BaseStream);
                }
                catch
                {
                    // Ignored because it may throw an exception.
                }

                if (pr.IsLastRequest)
                    _ps.EndPresence(pr, true);
            }
            catch (Exception ex)
            {
                Logger.Err(ex);
            }

            return Ok();
        }

        private async Task<ActionResult> RetOut(Stream stream)
        {
            stream.Position = 0;
            
            MemoryStream m = new MemoryStream();
            await stream.CopyToAsync(m);
            m.Position = 0;

            return File(m, "application/octet-stream");
        }
    }
}