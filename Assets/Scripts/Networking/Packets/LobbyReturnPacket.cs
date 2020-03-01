class LobbyReturnPacket : EmptyPacket
{
    override public PacketType Type()
    {
        return PacketType.LobbyReturn;
    }
}
