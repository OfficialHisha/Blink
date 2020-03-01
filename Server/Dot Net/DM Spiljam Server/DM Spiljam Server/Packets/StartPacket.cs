using Newtonsoft.Json;

namespace DM_Spiljam_Server.Packets
{
    public class StartPacket : Packet
    {
        class Data
        {
            public PacketType type;
        }

        public StartPacket(){}

        public string Dictify()
        {
            return JsonConvert.SerializeObject(new Data { type = Type() });
        }

        public PacketType Type()
        {
            return PacketType.Start;
        }
    }

}
