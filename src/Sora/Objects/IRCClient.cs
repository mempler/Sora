using System;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Sora.Database;
using Sora.Database.Models;
using Sora.Enums;
using Sora.EventArgs.BanchoEventArgs;
using Sora.Framework;
using Sora.Framework.Objects;
using Sora.Framework.Utilities;
using Sora.Services;

namespace Sora.Objects
{
    public class IrcClient
    {
        private TcpClient _client;
        private readonly SoraDbContext _ctx;
        private readonly IServerConfig _cfg;
        private readonly PresenceService _ps;
        private readonly IrcServer _parent;
        private readonly ChannelService _cs;
        private readonly EventManager _evmng;
        private bool _exit;

        public IrcClient(TcpClient client,
                         SoraDbContext ctx,
                         IServerConfig cfg,
                         PresenceService ps,
                         ChannelService cs,
                         EventManager evmng,
                         IrcServer parent)
        {
            _client = client;
            _ctx = ctx;
            _cfg = cfg;
            _ps = ps;
            _parent = parent;
            _cs = cs;
            _evmng = evmng;
        }

        private User _user;
        private Presence _presence;
        private bool _authorized;
        private string _nickname;
        private string _pass;

        private StreamWriter _writer;

        public void Start()
        {
            Receive().GetAwaiter().GetResult();
        }

        public void Stop()
        {
            _exit = true;
        }

        public void SendMessage(string message)
        {
            _writer.Write($":{_cfg.Server.Hostname} {message}\r\n");
        }

        public void SendCodeMessage(int code, string message, string nickname = "", string channel = "")
        {
            if (string.IsNullOrEmpty(nickname))
                nickname = _user?.UserName ?? _nickname;
            
            if (!string.IsNullOrEmpty(channel))
                channel += " " + channel;

            SendMessage($"{code:000} {nickname}:{channel} :{message}");
        }
        
        public async Task Receive()
        {
            var stream = _client.GetStream();
            var bytes = new byte[4096];

            _writer = new StreamWriter(stream);
            
            // TODO: Finish http://www.networksorcery.com/enp/protocol/irc.htm
            try
            {
                int i;
                while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
                {
                    if (_exit)
                    {
                        return;
                    }
                    
                    var data = Encoding.UTF8.GetString(bytes, 0, i).Split("\n");

                    foreach (var cmd in data.Select(d => d.Split(" "))
                                            .Where(cmd => !string.IsNullOrEmpty(cmd[0]))
                                            .Where(cmd => !string.IsNullOrWhiteSpace(cmd[0])))
                    {
                        switch (cmd[0])
                        {
                            case "USER":
                                if (_authorized)
                                {
                                    SendCodeMessage(462, "Unauthorized command (already authorized!)");
                                    continue;
                                }
                                
                                if (cmd.Length < 2)
                                {
                                    SendCodeMessage(451, "You have not registered");
                                    continue;
                                }

                                _nickname = cmd[1].Trim();
                                var rUser = DbUser.GetDbUser(_ctx, cmd[1].Trim()).Result;
                                if (rUser == null)
                                {
                                    SendCodeMessage(451, "You have not registered");
                                    continue;
                                }

                                _user = rUser.ToUser();

                                if (_authorized)
                                {
                                    SendCodeMessage(462, "Unauthorized command (already authorized!)");
                                    continue;
                                }

                                if (rUser?.IsPassword(Hex.ToHex(Crypto.GetMd5(_pass.Trim()))) != true)
                                {
                                    if (string.IsNullOrEmpty(_nickname))
                                        _nickname = "anonymous";
                                    
                                    Logger.Info(_pass);

                                    SendCodeMessage(464, "Password incorrect");
                                    continue;
                                }

                                _presence = new Presence(_user);
                                _presence.User = _user;
                                
                                _presence["STATUS"] = new UserStatus();
                                _presence["IRC"] = true;

                                WriteConnectionResponse();
                                WriteStatus();
                                WriteMotd();

                                _authorized = true;
                                break;
                            
                            case "JOIN":
                                if (_presence == null || !_authorized)
                                {
                                    SendCodeMessage(444, "You're not logged in");
                                    break;
                                }
                                
                                if (_cs.TryGet(cmd[1].Trim(), out var channel))
                                {
                                    SendCodeMessage(403, $"{cmd[1]}: No such Channel!");
                                    break;
                                }

                                channel.Join(_presence);
                                /*
                                if (!)
                                {
                                    SendCodeMessage(482, "You're not allowed to Join this Channel!");
                                    break;
                                }
                                */
                                
                                if (channel.Topic == "")
                                    SendCodeMessage(331, "No topic is set", channel: channel.Name);
                                else
                                    SendCodeMessage(332, channel.Topic, channel: channel.Name);

                                var uList = string.Join(
                                    " ", channel.Presences
                                                .Select(
                                                    ps =>
                                                    {
                                                        var ret = ps.User.UserName.Replace(" ", "_");

                                                        if (ps.User.Permissions.HasPermission(Permission.GROUP_CHAT_MOD))
                                                            ret = "@" + ret;
                                                        else if (ps.User.Permissions.HasPermission(
                                                            Permission.GROUP_DONATOR
                                                        ))
                                                            ret = "+" + ret;
                                                        
                                                        return ret;
                                                    })
                                                .ToArray()
                                );
                                uList += " @olSora";
                                SendCodeMessage(353, $"{uList}", channel: "= " + channel.Name);
                                SendCodeMessage(366, "End of NAMES list", channel: channel.Name);
                                break;
                            
                            case "PING":
                                if (_presence == null || !_authorized) {
                                    SendCodeMessage(444, "You're not logged in");
                                    break;
                                }

                                await _evmng.RunEvent(EventType.BanchoPong, new BanchoPongArgs {Pr = _presence});
                                
                                SendMessage($"PONG {_cfg.Server.Hostname}: {cmd[1]}");
                                break;
                            
                            case "PASS":
                                _pass = cmd[1];
                                break;
                            
                            case "QUIT":
                                _client.Close();
                                _parent.RemoveTcpClient(this);
                                return;

                            // Ignored Commands
                            case "NICK":
                            case "CAP":
                                break;
                            
                            default:
                                Logger.Debug(string.Join(" ", cmd));
                                if (!_authorized)
                                    SendCodeMessage(444, "You're not logged in");
                                break;
                        }
                    }

                    _writer.Flush();
                }
            }
            catch (Exception e)
            {
                Logger.Err(e);
                _client.Close();
            }
        }

        private void WriteConnectionResponse()
        {
            SendCodeMessage(1, "Welcome to the Internet Relay Network");
            SendCodeMessage(2, $"Your host is {_cfg.Server.Hostname}, running version olSora~UNKNOWN");
            SendCodeMessage(3, "This server was created 5/7/2019");
            SendCodeMessage(4, $"{_cfg.Server.Hostname} olSora~UNKNOWN o o");
        }

        private void WriteStatus()
        {
            SendCodeMessage(251, $"There are {_ps.ConnectedPresences} users and 0 services on 1 server");
        }
        
        private void WriteMotd()
        {
            SendCodeMessage(375, $"- {_cfg.Server.Hostname} Message of the day - ");
            SendCodeMessage(422, "MOTD File is missing"); // No MOTD for us :c
            SendCodeMessage(376, "End of MOTD command");
        }
    }
}
