using Newtonsoft.Json;

namespace DM_Spiljam_Server.Packets
{
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
}
