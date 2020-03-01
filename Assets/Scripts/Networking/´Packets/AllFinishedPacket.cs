using Newtonsoft.Json;
using System.Collections.Generic;

class AllFinishedPacket : Packet
{
    class Data
    {
        public PacketType type;
        public Dictionary<string, TimeStamp> scores;
    }

    public Dictionary<string, TimeStamp> Scores { get; }

    public AllFinishedPacket(Dictionary<string, TimeStamp> scores)
    {
        Scores = scores;
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
