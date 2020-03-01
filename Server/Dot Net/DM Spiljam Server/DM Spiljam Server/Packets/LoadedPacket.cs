using Newtonsoft.Json;

namespace DM_Spiljam_Server.Packets
{
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

}
