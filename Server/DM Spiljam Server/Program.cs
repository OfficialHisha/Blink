using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using DM_Spiljam_Server.Packets;
using Newtonsoft.Json.Linq;

namespace DM_Spiljam_Server
{
    public enum GamePhase
    {
        Lobby,
        Game,
        EndGame
    }

    class Program
    {
        private static TcpListener tcpListener;
        private static Thread listenThread;

        private static List<NetworkStream> clientConnections = new List<NetworkStream>();
        public static GamePhase GamePhase { get; set; } = GamePhase.Lobby;

        static void Main(string[] args)
        {
            tcpListener = new TcpListener(IPAddress.Any, 3000);
            listenThread = new Thread(new ThreadStart(ListenForClients));
            listenThread.Start();
        }

        static void ListenForClients()
        {
            tcpListener.Start();
            Console.WriteLine($"Server listening on {tcpListener.LocalEndpoint}");

            while (true)
            {
                //blocks until a client has connected to the server
                TcpClient client = tcpListener.AcceptTcpClient();

                //create a thread to handle communication 
                //with connected client
                Thread clientThread = new Thread(new ParameterizedThreadStart(HandleClientComm));
                clientThread.Start(client);
            }
        }

        static void HandleClientComm(object client)
        {
            TcpClient tcpClient = (TcpClient)client;
            NetworkStream clientStream = tcpClient.GetStream();

            Console.WriteLine($"Accepted client connection from {tcpClient.Client.RemoteEndPoint}");

            byte[] message = new byte[4096];
            int bytesRead;

            while (true)
            {
                try
                {
                    //blocks until a client sends a message
                    bytesRead = clientStream.Read(message, 0, 4096);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"ERROR: {e.Message}");
                    DropClient(clientStream);
                    break;
                }

                if (bytesRead == 0)
                {
                    Console.WriteLine("Client disconnected");
                    DropClient(clientStream);
                    break;
                }

                try
                {
                    string data = Encoding.UTF8.GetString(message);
                    string formattedData = data;
                    int index = data.IndexOf("}");
                    if (index > 0)
                        formattedData = data.Substring(0, index + 1);

                    Console.WriteLine(formattedData);

                    JObject json = JObject.Parse(formattedData);

                    if (!Enum.TryParse(json["type"].ToString(), out PacketType type))
                    {
                        Console.WriteLine("Invalid message received");
                        continue;
                    }

                    switch (type)
                    {
                        case PacketType.Handshake:
                            if (GamePhase != GamePhase.Lobby)
                            {
                                // Refuse connection as game is already started
                                Send(clientStream, new DisconnectPacket("Game in progress"));
                                continue;
                            }
                            clientConnections.Add(clientStream);

                            Lobby.AddLobbyClient(clientStream, json["name"].ToString());
                            break;
                        case PacketType.Ready:
                            if (GamePhase != GamePhase.Lobby)
                                continue;

                            Lobby.SetReady(clientStream, bool.Parse(json["ready"].ToString()));
                            break;
                        case PacketType.Loaded:
                            Game.SpawnEntity(clientStream);
                            break;
                        case PacketType.Relocation:
                            if (GamePhase != GamePhase.Game)
                                continue;

                            Broadcast(new RelocationPacket(int.Parse(json["entityId"].ToString()), float.Parse(json["x"].ToString()), float.Parse(json["y"].ToString())), new NetworkStream[] { clientStream });
                            break;
                        case PacketType.Visibility:
                            if (GamePhase != GamePhase.Game)
                                continue;

                            Broadcast(new VisibilityPacket(int.Parse(json["entityId"].ToString()), bool.Parse(json["visible"].ToString())), new NetworkStream[] { clientStream });
                            break;
                        case PacketType.Finish:
                            if (GamePhase != GamePhase.Game)
                                continue;

                            Game.AddFinishedEntity(clientStream, new TimeStamp(int.Parse(json["minutes"].ToString()), int.Parse(json["seconds"].ToString()), int.Parse(json["frames"].ToString())));
                            break;
                        case PacketType.LobbyReturn:
                            if (GamePhase != GamePhase.EndGame)
                                continue;

                            if (Lobby.ReturningClients.Contains(clientStream))
                                continue;

                            Lobby.ReturningClients.Add(clientStream);
                            if (Lobby.ReturningClients.Count == Lobby.LobbyClients.Count)
                            {
                                GamePhase = GamePhase.Lobby;
                                Broadcast(new LobbyStatePacket(Lobby.LobbyClients.Values.ToArray()));
                                Lobby.ReturningClients.Clear();
                            }
                            break;
                        default:
                            Console.WriteLine("Invalid packet type received");
                            continue;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"ERROR: {e.Message}");
                    continue;
                }
            }

            tcpClient.Close();
        }

        static void DropClient(NetworkStream client)
        {
            if (!clientConnections.Contains(client))
                // Client never properly connected
                return;

            if (GamePhase == GamePhase.Game)
                Game.DestroyEntityForClient(client);

            if (Lobby.ReturningClients.Contains(client))
                Lobby.ReturningClients.Remove(client);
            Lobby.RemoveLobbyClient(client);


            clientConnections.Remove(client);

            if (clientConnections.Count == 0)
                GamePhase = GamePhase.Lobby;
        }

        public static void Send(NetworkStream client, Packet packet)
        {
            Console.WriteLine($"Sending packet {packet.Dictify()}");

            byte[] data = Encoding.UTF8.GetBytes(packet.Dictify());
            client.Write(data, 0, data.Length);
        }

        public static void Broadcast(Packet packet, NetworkStream[] excludedClients = null)
        {
            NetworkStream[] toClients = clientConnections.ToArray();
            if (excludedClients != null)
                toClients = clientConnections.Where(x => !excludedClients.Contains(x)).ToArray();

            foreach (NetworkStream client in toClients)
            {
                Send(client, packet);
            }
        }
    }
}
