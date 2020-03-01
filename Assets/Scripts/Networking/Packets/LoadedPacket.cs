class LoadedPacket : EmptyPacket
{
    override public PacketType Type()
    {
        return PacketType.Loaded;
    }
}
