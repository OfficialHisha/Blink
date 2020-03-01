namespace DM_Spiljam_Server.Packets
{
    class StartPacket : EmptyPacket
    {
        override public PacketType Type()
        {
            return PacketType.Start;
        }
    }

}
