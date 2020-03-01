using Newtonsoft.Json;

namespace DM_Spiljam_Server.Packets
{
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
        public float Y { get; }

        public RelocationPacket(int entityId, float x, float y)
        {
            EntityId = entityId;
            X = x;
            Y = y;
        }

        public string Dictify()
        {
            return JsonConvert.SerializeObject(new Data { type = Type(), entityId = EntityId, x = X, y = Y});
        }

        public PacketType Type()
        {
            return PacketType.Relocation;
        }
    }
}
