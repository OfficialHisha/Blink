namespace DM_Spiljam_Server.Packets
{
    class LobbyReturnPacket : EmptyPacket
    {
        override public PacketType Type()
        {
            return PacketType.LobbyReturn;
        }
    }
}
