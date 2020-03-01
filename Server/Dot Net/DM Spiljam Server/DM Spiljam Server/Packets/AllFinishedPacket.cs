using Newtonsoft.Json;
using System.Collections.Generic;

namespace DM_Spiljam_Server.Packets
{
    class AllFinishedPacket : Packet
    {
        class Data
        {
            public PacketType type;
            public Dictionary<string, TimeStamp> scores;
        }

        public Dictionary<string, TimeStamp> Scores { get; }

        public AllFinishedPacket(List<KeyValuePair<string, TimeStamp>> scores)
        {
            Scores = new Dictionary<string, TimeStamp>(scores);
        }

        public string Dictify()
        {
            return JsonConvert.SerializeObject(new Data { type = Type(), scores = Scores });
        }

        public PacketType Type()
        {
            return PacketType.AllFinished;
        }
    }
}
