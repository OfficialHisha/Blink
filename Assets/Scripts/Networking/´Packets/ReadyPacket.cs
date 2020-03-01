using Newtonsoft.Json;

public class ReadyPacket : Packet
{
    class Data
    {
        public PacketType type;
        public bool ready;
    }

    public bool Ready { get; }

    public ReadyPacket(bool ready)
    {
        Ready = ready;
    }

    public string Dictify()
    {
        return JsonConvert.SerializeObject(new Data { type = Type(), ready = Ready });
    }

    public PacketType Type()
    {
        return PacketType.Ready;
    }
}
