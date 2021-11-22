using LightweightServer.Client;
using LightweightServer.Server;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace LightweightServer
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Change these values to whatever you want.
            IPAddress hostAddr = IPAddress.Loopback;
            int port = 27000;

            LobbyServer server = new LobbyServer(hostAddr, port, new ClientHandler(new LobbyManager()));
            await server.Start();
        }

        private static async Task Example()
        {
            // Example of two clients talking to each other  through a our relay server.
            LobbyServer server = new LobbyServer(IPAddress.Loopback, 27000, new ClientHandler(new LobbyManager()));
            server.Start();
            LobbyClient lobbyClient1 = new LobbyClient(IPAddress.Loopback, 27000);
            await lobbyClient1.CreateLobbyAsync("A");
            LobbyClient lobbyClient2 = new LobbyClient(IPAddress.Loopback, 27000);
            await lobbyClient2.JoinLobbyAsync("A");
            await lobbyClient2.RelayTextAsync("A", "Hello!");
            Console.WriteLine(await lobbyClient1.ReceiveRelayedTextAsync());
            await lobbyClient1.RelayTextAsync("A", "haha we are relaying messages!");
            Console.WriteLine(await lobbyClient2.ReceiveRelayedTextAsync());
        }
    }
}
