using DM_Spiljam_Server.Packets;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;

namespace DM_Spiljam_Server
{
    static class Game
    {
        static Dictionary<NetworkStream, Entity> entityMap = new Dictionary<NetworkStream, Entity>();
        static Dictionary<string, TimeStamp> finishedPlayers = new Dictionary<string, TimeStamp>();

        public static void SpawnEntity(NetworkStream client)
        {
            string owner = Lobby.LobbyClients[client].Name;

            Entity ent = new Entity(Entity.NextEntityId, owner);
            entityMap.Add(client, ent);

            Program.Send(client, new SpawnEntityPacket(ent.EntityId, owner, 0, 0, true));

            foreach (Entity entity in entityMap.Values)
            {
                if (entity.Equals(ent))
                    // Don't add ourselves again
                    continue;

                Program.Send(client, new SpawnEntityPacket(entity.EntityId, owner, entity.X, entity.Y, false));
            }

            Program.Broadcast(new SpawnEntityPacket(ent.EntityId, owner, 0, 0, false), new NetworkStream[] { client });

            if (entityMap.Count == Lobby.LobbyClients.Count)
                Program.Broadcast(new AllLoadedPacket());
        }

        public static void DestroyEntityForClient(NetworkStream client)
        {
            Program.Broadcast(new RemoveEntityPacket(entityMap[client].EntityId), new NetworkStream[] { client });
            finishedPlayers.Remove(entityMap[client].Owner);
            entityMap.Remove(client);
        }

        public static void AddFinishedEntity(NetworkStream client, TimeStamp finishTime)
        {
            finishedPlayers.Add(entityMap[client].Owner, finishTime);

            if (finishedPlayers.Count == entityMap.Count)
            {
                // Everyone has finished
                List<KeyValuePair<string, TimeStamp>> myList = finishedPlayers.ToList();
                Program.Broadcast(new AllFinishedPacket(myList.OrderBy(x => x.Value.Minutes).ThenBy(x => x.Value.Seconds).ThenBy(x => x.Value.Frames).ToList()));
            }
        }
    }
}
