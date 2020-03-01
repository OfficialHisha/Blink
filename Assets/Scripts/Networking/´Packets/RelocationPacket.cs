using Newtonsoft.Json;
using UnityEngine;

class RelocationPacket : Packet
{
    class Data
    {
        public PacketType type;
        public int entityId;
        public float x;
        public float y;
    }

    public int EntityId { get; }
    public float X { get; }
    public float Y { get; set; }

    public RelocationPacket(int entityId, float x, float y)
    {
        EntityId = entityId;
        X = x;
        Y = y;
    }

    public RelocationPacket(int entityId, Vector3 newPosition) : this(entityId, newPosition.x, newPosition.y){}

    public string Dictify()
    {
        return JsonConvert.SerializeObject(new Data { type = Type(), entityId = EntityId, x = X, y = Y });
    }

    public PacketType Type()
    {
        return PacketType.Relocation;
    }
}
