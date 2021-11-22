using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LightweightServer.Server
{
    class LobbyManager
    {
        public Dictionary<string, List<TcpClient>> rooms;

        public LobbyManager()
        {
            rooms = new Dictionary<string, List<TcpClient>>();
        }

        public void Create(string name, TcpClient client)
        {
            rooms[name] = new List<TcpClient>() { client };
        }

        public void Join(string name, TcpClient tcpClient)
        {
            rooms[name].Add(tcpClient);
        }

        public void Leave(string name, TcpClient tcpClient)
        {
            rooms[name].Remove(tcpClient);
        }

        public async Task Relay(string name, byte[] bytes, TcpClient sender)
        {
            ValueTask[] relays = new ValueTask[rooms[name].Count];

            int i = 0;
            byte[] withHeader = new byte[4 + bytes.Length];
            BitConverter.GetBytes(bytes.Length).CopyTo(withHeader, 0);
            bytes.CopyTo(withHeader, 4);
            foreach (TcpClient client in rooms[name])
            {
                if (client == sender) continue;

                relays[i] = client.GetStream().WriteAsync(withHeader);
                i++;
            }

            foreach (ValueTask item in relays)
            {
                await item;
            }            
        }
    }
}
