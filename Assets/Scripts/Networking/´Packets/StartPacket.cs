using Newtonsoft.Json;

public class StartPacket : Packet
{
    class Data
    {
        public PacketType type;
    }

    public StartPacket(){}

    public string Dictify()
    {
        return JsonConvert.SerializeObject(new Data { type = Type() });
    }

    public PacketType Type()
    {
        return PacketType.Start;
    }
}
