class StartPacket : EmptyPacket
{
    override public PacketType Type()
    {
        return PacketType.Start;
    }
}
