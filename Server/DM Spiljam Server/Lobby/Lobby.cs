using DM_Spiljam_Server.Packets;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace DM_Spiljam_Server
{
    static class Lobby
    {
        public static Dictionary<NetworkStream, LobbyClient> LobbyClients { get; } = new Dictionary<NetworkStream, LobbyClient>();
        public static List<NetworkStream> ReturningClients { get; } = new List<NetworkStream>();

        public static void AddLobbyClient(NetworkStream client, string name)
        {
            System.Console.WriteLine($"Adding {name} to lobby");

            LobbyClient newClient = new LobbyClient(name);
            LobbyClients.Add(client, newClient);

            Program.Broadcast(new LobbyStatePacket(LobbyClients.Values.ToArray()));
        }

        public static void SetReady(NetworkStream client, bool ready)
        {
            LobbyClients[client].Ready = ready;

            Program.Broadcast(new LobbyStatePacket(LobbyClients.Values.ToArray()));

            if (LobbyClients.Values.All(c => c.Ready))
            {
                foreach (LobbyClient player in LobbyClients.Values)
                    player.Ready = false;

                Program.GamePhase = GamePhase.Game;
                Program.Broadcast(new StartPacket());
            }
                
        }

        public static void RemoveLobbyClient(NetworkStream client)
        {
            if (!LobbyClients.ContainsKey(client))
                return;

            LobbyClients.Remove(client);
        }
    }
}
