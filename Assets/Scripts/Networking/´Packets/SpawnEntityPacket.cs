using Newtonsoft.Json;
using UnityEngine;

class SpawnEntityPacket : Packet
{
    class Data
    {
        public PacketType type;
        public int entityId;
        public string owner;
        public float x;
        public float y;
        public bool isSelf;
    }

    public int EntityId { get; }
    public string Owner { get; }
    public float X { get; }
    public float Y { get; }
    public bool IsSelf { get; }

    public SpawnEntityPacket(int entityId, string owner, float x, float y, bool isSelf)
    {
        EntityId = entityId;
        Owner = owner;
        X = x;
        Y = y;
        IsSelf = isSelf;
    }
    public SpawnEntityPacket(int entityId, string owner, Vector3 position, bool isSelf) : this(entityId, owner, position.x, position.y, isSelf){}

    public string Dictify()
    {
        return JsonConvert.SerializeObject(new Data { type = Type(), entityId = EntityId, owner = Owner, x = X, y = Y, isSelf = IsSelf });
    }

    public PacketType Type()
    {
        return PacketType.SpawnEntity;
    }
}
