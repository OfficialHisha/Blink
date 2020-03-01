using Newtonsoft.Json;

namespace DM_Spiljam_Server.Packets
{
    public class DisconnectPacket : Packet
    {
        class Data
        {
            public PacketType type;
            public string reason;
        }

        public string Reason { get; }

        public DisconnectPacket(string reason)
        {
            Reason = reason;
        }

        public string Dictify()
        {
            return JsonConvert.SerializeObject(new Data { type = Type(), reason = Reason});
        }

        public PacketType Type()
        {
            return PacketType.Disconnect;
        }
    }
}