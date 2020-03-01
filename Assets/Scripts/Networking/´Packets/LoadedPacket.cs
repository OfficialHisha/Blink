using Newtonsoft.Json;

public class LoadedPacket : Packet
{
    class Data
    {
        public PacketType type;
    }

    public LoadedPacket(){}

    public string Dictify()
    {
        return JsonConvert.SerializeObject(new Data { type = Type() });
    }

    public PacketType Type()
    {
        return PacketType.Loaded;
    }
}
