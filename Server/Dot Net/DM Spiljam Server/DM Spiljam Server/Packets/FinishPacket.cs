using Newtonsoft.Json;

namespace DM_Spiljam_Server.Packets
{
    class FinishPacket
    {
        class Data
        {
            public PacketType type;
            public int minutes;
            public int seconds;
            public int frames;
        }

        public int Minutes { get; }
        public int Seconds { get; }
        public int Frames { get; }

        public FinishPacket(int minutes, int seconds, int frames)
        {
            Minutes = minutes;
            Seconds = seconds;
            Frames = frames;
        }

        public FinishPacket(TimeStamp finishTime) : this(finishTime.Minutes, finishTime.Seconds, finishTime.Frames){}

        public string Dictify()
        {
            return JsonConvert.SerializeObject(new Data { type = Type(), minutes =Minutes, seconds = Seconds, frames = Frames });
        }

        public PacketType Type()
        {
            return PacketType.Finish;
        }
    }
}
