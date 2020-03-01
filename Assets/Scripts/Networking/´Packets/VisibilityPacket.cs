using Newtonsoft.Json;

public class VisibilityPacket : Packet
{
    class Data
    {
        public PacketType type;
        public int entityId;
        public bool visible;
    }

    public int EntityId { get; }

    public bool Visible { get; set; }

    public VisibilityPacket(int entityId, bool visible)
    {
        EntityId = entityId;
        Visible = visible;
    }

    public string Dictify()
    {
        return JsonConvert.SerializeObject(new Data { type = Type(), entityId = EntityId, visible = Visible });
    }

    public PacketType Type()
    {
        return PacketType.Visibility;
    }
}
