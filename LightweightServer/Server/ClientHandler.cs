using LightweightServer.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LightweightServer.Server
{
    class ClientHandler
    {
        private readonly LobbyManager _lobbyManager;

        public ClientHandler(LobbyManager lobbyManager)
        {
            _lobbyManager = lobbyManager;
        }

        public async Task Handle(TcpClient client)
        {
            Console.WriteLine("Handling new client");
            NetworkStream network = client.GetStream();
            byte[] sizeBytes = new byte[4];
            while (true)
            {
                byte[] msg = await ReadMessage(sizeBytes, network);

                LobbyCommand lobbyCommand = (LobbyCommand)msg[0];
                string lobbyName;
                switch (lobbyCommand)
                {
                    case LobbyCommand.Create:
                        lobbyName = Encoding.UTF8.GetString(msg, 1, msg.Length - 1);
                        _lobbyManager.Create(lobbyName, client);
                        Console.WriteLine("Created lobby " + lobbyName);
                        break;

                    case LobbyCommand.Join:
                        lobbyName = Encoding.UTF8.GetString(msg, 1, msg.Length - 1);
                        _lobbyManager.Join(lobbyName, client);
                        Console.WriteLine("Joined lobby " + lobbyName);
                        break;

                    case LobbyCommand.Leave:
                        lobbyName = Encoding.UTF8.GetString(msg, 1, msg.Length - 1);
                        _lobbyManager.Leave(lobbyName, client);
                        Console.WriteLine("Left lobby " + lobbyName);
                        break;

                    case LobbyCommand.Relay:
                        int lobbyNameLength = BitConverter.ToInt32(msg, 1);
                        lobbyName = Encoding.UTF8.GetString(msg, 5, lobbyNameLength);
                        await _lobbyManager.Relay(lobbyName, msg, client);
                        Console.WriteLine("Relayed messages to " + lobbyName);
                        break;
                }
            }
        }

        public async Task<byte[]> ReadMessage(byte[] sizeBytes, NetworkStream network)
        {
            await network.ReadAsync(sizeBytes);

            int length = BitConverter.ToInt32(sizeBytes);
            byte[] message = new byte[length];

            await network.ReadAsync(message);
            return message;
        }
    }
}
