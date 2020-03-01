namespace DM_Spiljam_Server
{
    struct TimeStamp
    {
        public int Minutes { get; }
        public int Seconds { get; }
        public int Frames { get; }

        public TimeStamp(int minutes, int seconds, int frames)
        {
            Minutes = minutes;
            Seconds = seconds;
            Frames = frames;
        }

        public override string ToString()
        {
            return $"{Minutes.ToString("00")}:{Seconds.ToString("00")}:{Frames.ToString("000")}";
        }
    }
}
