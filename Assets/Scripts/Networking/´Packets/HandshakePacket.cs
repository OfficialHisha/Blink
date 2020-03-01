using Newtonsoft.Json;

public class HandshakePacket : Packet
{
    class Data
    {
        public PacketType type;
        public string name;
    }

    public string Name { get; }

    public HandshakePacket(string name)
    {
        Name = name;
    }

    public string Dictify()
    {
        return JsonConvert.SerializeObject(new Data { type = Type(), name = Name });
    }

    public PacketType Type()
    {
        return PacketType.Handshake;
    }
}
