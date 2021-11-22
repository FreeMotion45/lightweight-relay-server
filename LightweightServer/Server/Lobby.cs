using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LightweightServer.Server
{
    class Lobby
    {
        private List<TcpClient> _clients;

        public Lobby()
        {
            _clients = new List<TcpClient>();
        }

        public void AddClient(TcpClient client)
        {
            _clients.Add(client);
        }

        public void RemoveClient(TcpClient client)
        {
            _clients.Remove(client);
        }

        public async Task Publish(byte[] bytes)
        {
            List<ValueTask> tasks = new List<ValueTask>();
            foreach (TcpClient client in _clients)
            {
                tasks.Add(client.GetStream().WriteAsync(bytes));
            }
            foreach (ValueTask task in tasks)
            {
                await task;
            }
        }
    }
}
