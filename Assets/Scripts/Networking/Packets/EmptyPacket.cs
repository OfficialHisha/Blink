using Newtonsoft.Json;

abstract class EmptyPacket : Packet
{
    class Data
    {
        public PacketType type;
    }

    public string Dictify()
    {
        return JsonConvert.SerializeObject(new Data { type = Type() });
    }

    public abstract PacketType Type();
}
