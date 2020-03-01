using Newtonsoft.Json;

namespace DM_Spiljam_Server.Packets
{
    class RemoveEntityPacket : Packet
    {
        class Data
        {
            public PacketType type;
            public int entityId;
        }

        public int EntityId { get; }

        public RemoveEntityPacket(int entityId)
        {
            EntityId = entityId;
        }

        public string Dictify()
        {
            return JsonConvert.SerializeObject(new Data { type = Type(), entityId = EntityId });
        }

        public PacketType Type()
        {
            return PacketType.RemoveEntity;
        }
    }
}
