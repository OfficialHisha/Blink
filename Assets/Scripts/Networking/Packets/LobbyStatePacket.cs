using Newtonsoft.Json;

class LobbyStatePacket : Packet
{
    class Data
    {
        public PacketType type;
        public LobbyClient[] players;
    }

    public LobbyClient[] Players { get; }

    public LobbyStatePacket(LobbyClient[] players)
    {
        Players = players;
    }

    public string Dictify()
    {
        return JsonConvert.SerializeObject(new Data { type = Type(), players = Players });
    }

    public PacketType Type()
    {
        return PacketType.LobbyState;
    }
}
