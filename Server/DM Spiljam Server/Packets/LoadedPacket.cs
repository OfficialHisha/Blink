using Newtonsoft.Json;

namespace DM_Spiljam_Server.Packets
{
    class LoadedPacket : EmptyPacket
    {
        override public PacketType Type()
        {
            return PacketType.Loaded;
        }
    }

}
