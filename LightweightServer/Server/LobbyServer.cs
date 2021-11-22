using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LightweightServer.Server
{
    class LobbyServer
    {
        private readonly ClientHandler _clientHandler;
        private TcpListener _listener;

        public LobbyServer(IPAddress addr, int port, ClientHandler clientHandler)
        {
            _listener = new TcpListener(addr, port);
            _clientHandler = clientHandler;
        }

        public bool Running { get; set; }

        public async Task Start()
        {
            Running = true;
            _listener.Start();
            while (Running)
            {
                TcpClient client = await _listener.AcceptTcpClientAsync();
                _clientHandler.Handle(client);
            }
        }
    }
}
