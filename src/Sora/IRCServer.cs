using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Sora.Database;
using Sora.Helpers;
using Sora.Objects;
using Sora.Services;

namespace Sora.Server
{
    public class IRCServer
    {
        private readonly IServerConfig _cfg;
        //private readonly SoraDbContextFactory _factory;
        private readonly PresenceService _ps;
        private readonly ChannelService _cs;
        private readonly EventManager _evmng;
        private CancellationTokenSource _token;
        private TcpListener _listener;
        private List<IRCClient> _connectedClients;

        private object conLocker;

        public IRCServer(IServerConfig cfg,
            SoraDbContextFactory factory,
            PresenceService ps,
            ChannelService cs,
            EventManager evmng)
        {
            _cfg = cfg;
            _factory = factory;
            _ps = ps;
            _cs = cs;
            _evmng = evmng;

            IPAddress ipAddress;
            if (!IPAddress.TryParse(cfg.Server.Hostname, out ipAddress))
                ipAddress = Dns.GetHostEntry(cfg.Server.Hostname).AddressList[0];
            _listener = new TcpListener(ipAddress, cfg.Server.IRCPort);

            _token = new CancellationTokenSource();
            _connectedClients = new List<IRCClient>();
            conLocker = new object();
        }
        public async void StartAsync()
        {
            await Task.Run(
                () =>
                {
                    _listener.Start();
                    var isCanceled = false;
                    
                    _token.Token.Register(
                        () => isCanceled = true
                    );

                    while (!isCanceled)
                    {
                        try
                        {
                            var client = _listener.AcceptTcpClient();
                            
                            var ircClient = new IRCClient(client, _factory, _cfg, _ps, _cs, _evmng, this);
                            lock(conLocker)
                                _connectedClients.Add(ircClient);
                            new Thread(ircClient.Start).Start();
                        }
                        catch (Exception ex)
                        {
                            Logger.Err(ex);
                        }
                    }

                    lock (conLocker)
                    {
                        foreach (var c in _connectedClients)
                        {
                            c.Stop();
                        }
                    }
                }
            );
        }

        public void RemoveTCPClient(IRCClient client)
        {
            lock(conLocker)
                _connectedClients.Remove(client);
        }
        
        public void Stop() => _token.Cancel();
    }
}
