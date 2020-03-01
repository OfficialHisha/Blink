using Newtonsoft.Json;

class AllLoadedPacket : Packet
{
    class Data
    {
        public PacketType type;
    }

    public AllLoadedPacket() { }

    public string Dictify()
    {
        return JsonConvert.SerializeObject(new Data { type = Type() });
    }

    public PacketType Type()
    {
        return PacketType.AllLoaded;
    }
}
