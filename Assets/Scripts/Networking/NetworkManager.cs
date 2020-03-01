using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManager : MonoBehaviour
{
    public static NetworkManager instance;

    public GameObject entityPrefab;

    TcpClient tcpClient;
    NetworkStream serverStream;

    Dictionary<int, GameObject> entityMap = new Dictionary<int, GameObject>();

    Scene currentScene;
    string requestedScene;
    bool loadingScene = false;

    object spawnQueueLock = new object();
    Queue<SpawnEntityPacket> spawnQueue = new Queue<SpawnEntityPacket>();

    object relocLock = new object();
    Queue<RelocationPacket> relocationQueue = new Queue<RelocationPacket>();

    object dropLock = new object();
    Queue<RemoveEntityPacket> dropQueue = new Queue<RemoveEntityPacket>();

    object visibilityLock = new object();
    Queue<VisibilityPacket> visibilityQueue = new Queue<VisibilityPacket>();

    public LobbyClient[] lobbyClients;

    public bool connected = false;

    public bool allLoaded = false;

    public bool allFinished = false;
    public Dictionary<string, TimeStamp> scores = new Dictionary<string, TimeStamp>();

    void Awake()
    {
        currentScene = SceneManager.GetActiveScene();
        requestedScene = currentScene.name;

        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (spawnQueue.Count > 0)
        {
            lock (spawnQueueLock)
            {
                SpawnEntityPacket packet = spawnQueue.Dequeue();
                GameObject entity = Instantiate(entityPrefab, new Vector3(packet.X, packet.Y), Quaternion.identity);
                entity.GetComponent<NetworkEntity>().Setup(packet.EntityId, packet.IsSelf, lobbyClients.Where(x => x.Name == packet.Owner).SingleOrDefault());

                entityMap.Add(packet.EntityId, entity);

                Debug.Log($"Spawned entity: {packet.EntityId} - Self: {packet.IsSelf}");
                GameObject.Find("RoomManager").GetComponent<RoomManager>().players++;
            }
        }

        if (relocationQueue.Count > 0)
        {
            lock(relocLock)
            {
                RelocationPacket packet = relocationQueue.Dequeue();
                entityMap[packet.EntityId].transform.position = new Vector3(packet.X, packet.Y);
            }
        }

        if (dropQueue.Count > 0)
        {
            lock(dropLock)
            {
                RemoveEntityPacket packet = dropQueue.Dequeue();
                Destroy(entityMap[packet.EntityId]);
                entityMap.Remove(packet.EntityId);
            }
        }

        if (visibilityQueue.Count > 0)
        {
            lock (visibilityLock)
            {
                VisibilityPacket packet = visibilityQueue.Dequeue();
                entityMap[packet.EntityId].GetComponent<SpriteRenderer>().enabled = packet.Visible;
            }
        }

        if (!loadingScene && currentScene.name != requestedScene)
        {
            loadingScene = true;

            Debug.Log($"Loading '{requestedScene}' from '{currentScene.name}'");
            SceneManager.LoadScene(requestedScene);
            currentScene = SceneManager.GetSceneByName(requestedScene);

            loadingScene = false;
        }

        if (allLoaded)
        {
            allLoaded = false;
            GameObject.Find("RoomManager").GetComponent<RoomManager>().AllLoaded();
        }
        if (allFinished)
        {
            allFinished = false;
            GameObject.Find("RoomManager").GetComponent<RoomManager>().AllFinished();
        }
    }

    void OnApplicationQuit()
    {
        tcpClient?.Close();
    }

    public void Connect(string hostname = "localhost", int port = 3000)
    {
        try
        {
            tcpClient = new TcpClient(hostname, port);

            //create a thread to handle communication 
            //with connected client
            Thread serverThread = new Thread(new ParameterizedThreadStart(HandleServerComm));
            serverThread.Start(tcpClient);
        }
        catch (SocketException e)
        {
            Debug.LogError($"Error connecting to server: {e.Message}");
        }
    }

    public void Disconnect()
    {
        connected = false;
        tcpClient?.Close();
        requestedScene = "MainMenu";
    }

    public void Send(Packet packet)
    {
        byte[] test = Encoding.UTF8.GetBytes(packet.Dictify());
        serverStream.Write(test, 0, test.Length);
    }

    void HandleServerComm(object server)
    {
        TcpClient tcpClient = (TcpClient)server;
        serverStream = tcpClient.GetStream();
        connected = true;
        Debug.Log($"Connected to server on {tcpClient.Client.RemoteEndPoint}");

        byte[] message = new byte[4096];
        int bytesRead;

        while (true)
        {
            try
            {
                Debug.Log("Waiting for server");
                //blocks until the server sends a message
                bytesRead = serverStream.Read(message, 0, 4096);
            }
            catch (Exception e)
            {
                Debug.LogError($"ERROR: {e.Message}");
                break;
            }

            if (bytesRead == 0)
            {
                Debug.LogWarning("Lost connection to server");
                break;
            }

            try
            {
                string formattedData = FormatJsonString(Encoding.UTF8.GetString(message));
                Debug.Log(formattedData);
                JObject json = JObject.Parse(formattedData);

                if (!Enum.TryParse(json["type"].ToString(), out PacketType type))
                {
                    Console.WriteLine("Invalid message received");
                    continue;
                }

                switch (type)
                {
                    case PacketType.Disconnect:
                        Debug.LogWarning($"Disconnected from host with message: {json["reason"].ToString()}");
                        break;
                    case PacketType.LobbyState:
                        requestedScene = "Lobby";

                        lobbyClients = JsonConvert.DeserializeObject<LobbyClient[]>(json["players"].ToString());
                        break;
                    case PacketType.Start:
                        requestedScene = "Game 1";
                        break;
                    case PacketType.SpawnEntity:
                        SpawnEntityPacket SpawnPacket = new SpawnEntityPacket(int.Parse(json["entityId"].ToString()), json["owner"].ToString(), float.Parse(json["x"].ToString()), float.Parse(json["y"].ToString()), bool.Parse(json["isSelf"].ToString()));

                        lock (spawnQueueLock)
                            spawnQueue.Enqueue(SpawnPacket);

                        break;
                    case PacketType.AllLoaded:
                        allLoaded = true;
                        break;
                    case PacketType.RemoveEntity:
                        RemoveEntityPacket removePacket = new RemoveEntityPacket(int.Parse(json["entityId"].ToString()));

                        lock (dropLock)
                            dropQueue.Enqueue(removePacket);

                        break;
                    case PacketType.Relocation:
                        Vector3 newPosition = new Vector3(float.Parse(json["x"].ToString()), float.Parse(json["y"].ToString()));
                        RelocationPacket relocPacket = new RelocationPacket(int.Parse(json["entityId"].ToString()), newPosition);

                        lock (relocLock)
                            relocationQueue.Enqueue(relocPacket);

                        break;
                    case PacketType.Visibility:
                        VisibilityPacket visibilityPacket = new VisibilityPacket(int.Parse(json["entityId"].ToString()), bool.Parse(json["visible"].ToString()));

                        lock (visibilityLock)
                            visibilityQueue.Enqueue(visibilityPacket);

                        break;
                    case PacketType.AllFinished:
                        scores = json["scores"].ToObject<Dictionary<string, TimeStamp>>();
                        allFinished = true;
                        break;
                    default:
                        Debug.LogWarning("Invalid packet type received");
                        continue;
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                continue;
            }
        }

        Disconnect();
    }

    string FormatJsonString(string json)
    {
        int openCurlies = 0;
        int closeCurlies = 0;

        int i = 0;
        foreach (char character in json)
        {
            if (character == '{')
                openCurlies++;
            else if (character == '}')
                closeCurlies++;

            i++;

            if (closeCurlies == openCurlies)
            {
                return json.Substring(0, i);
            }
        }

        return "";
    }
}
