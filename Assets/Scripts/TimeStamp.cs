public struct TimeStamp
{
    public int Minutes { get; set; }
    public int Seconds { get; set; }
    public int Frames { get; set; }

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
