using LightweightServer.Common;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LightweightServer.Client
{
    public class LobbyClient
    {
        private TcpClient client;

        public LobbyClient(IPAddress addr, int port)
        {
            client = new TcpClient();
            client.Connect(addr, port);
        }

        public async Task CreateLobbyAsync(string name)
        {
            byte[] msg = new byte[name.Length + 1];
            msg[0] = (byte)LobbyCommand.Create;
            Encoding.UTF8.GetBytes(name, 0, name.Length, msg, 1);
            await client.GetStream().WriteAsync(PackMessage(msg));
        }

        public async Task LeaveLobbyAsync(string name)
        {
            byte[] msg = new byte[name.Length + 1];
            msg[0] = (byte)LobbyCommand.Leave;
            Encoding.UTF8.GetBytes(name, 0, name.Length, msg, 1);
            await client.GetStream().WriteAsync(PackMessage(msg));
        }

        public async Task JoinLobbyAsync(string name)
        {
            byte[] msg = new byte[name.Length + 1];
            msg[0] = (byte)LobbyCommand.Join;
            Encoding.UTF8.GetBytes(name, 0, name.Length, msg, 1);
            await client.GetStream().WriteAsync(PackMessage(msg));
        }

        public async Task RelayAsync(string name, byte[] bytes)
        {
            byte[] msg = new byte[1 + 4 + name.Length + bytes.Length];
            msg[0] = (byte)LobbyCommand.Relay;
            BitConverter.GetBytes(name.Length).CopyTo(msg, 1);
            Encoding.UTF8.GetBytes(name, 0, name.Length, msg, 1 + 4);
            bytes.CopyTo(msg, 1 + 4 + name.Length);
            await client.GetStream().WriteAsync(PackMessage(msg));
        }

        public async Task RelayTextAsync(string name, string text)
        {
            await RelayAsync(name, Encoding.UTF8.GetBytes(text));
        }

        public async Task<byte[]> ReceiveRelayedMessageAsync()
        {
            byte[] msg = await ReadMessageAsync(new byte[4], client.GetStream());
            byte[] nmLen = msg.Take(5).ToArray();            
            int nml = BitConverter.ToInt32(nmLen, 1);
            byte[] nm = msg.Skip(5).Take(nml).ToArray();
            byte[] amsg = msg.Skip(5+nml).ToArray();
            return amsg;
        }

        public async Task<string> ReceiveRelayedTextAsync()
        {
            return Encoding.UTF8.GetString(await ReceiveRelayedMessageAsync());
        }

        private async Task<byte[]> ReadMessageAsync(byte[] sizeBytes, NetworkStream network)
        {
            await network.ReadAsync(sizeBytes);

            int length = BitConverter.ToInt32(sizeBytes);
            byte[] message = new byte[length];

            await network.ReadAsync(message);
            return message;
        }

        private byte[] PackMessage(byte[] b)
        {
            byte[] bb = new byte[4 + b.Length];
            BitConverter.GetBytes(b.Length).CopyTo(bb, 0);
            b.CopyTo(bb, 4);
            return bb;
        }
    }
}
