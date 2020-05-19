using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Sora.Database;
using Sora.Framework.Utilities;
using Sora.Objects;
using Sora.Services;

namespace Sora
{
    public class IrcServer
    {
        private readonly IServerConfig _cfg;

        private readonly SoraDbContextFactory _factory;

        //private readonly SoraDbContextFactory _factory;
        private readonly PresenceService _ps;
        private readonly ChannelService _cs;
        private readonly EventManager _evmng;
        private CancellationTokenSource _token;
        private TcpListener _listener;
        private List<IrcClient> _connectedClients;

        private object _conLocker;

        public IrcServer(IServerConfig cfg,
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
            _listener = new TcpListener(ipAddress, cfg.Server.IrcPort);

            _token = new CancellationTokenSource();
            _connectedClients = new List<IrcClient>();
            _conLocker = new object();
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
                            
                            var ircClient = new IrcClient(client, _factory, _cfg, _ps, _cs, _evmng, this);
                            lock(_conLocker)
                                _connectedClients.Add(ircClient);
                            new Thread(ircClient.Start).Start();
                        }
                        catch (Exception ex)
                        {
                            Logger.Err(ex);
                        }
                    }

                    lock (_conLocker)
                    {
                        foreach (var c in _connectedClients)
                        {
                            c.Stop();
                        }
                    }
                }
            );
        }

        public void RemoveTcpClient(IrcClient client)
        {
            lock(_conLocker)
                _connectedClients.Remove(client);
        }
        
        public void Stop() => _token.Cancel();
    }
}
