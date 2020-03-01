class AllFinishedPacket : EmptyPacket
{
    override public PacketType Type()
    {
        return PacketType.AllFinished;
    }
}
