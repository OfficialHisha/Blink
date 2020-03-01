namespace DM_Spiljam_Server.Packets
{
    class AllLoadedPacket : EmptyPacket
    {
        override public PacketType Type()
        {
            return PacketType.AllLoaded;
        }
    }
}
