namespace DM_Spiljam_Server.Packets
{
    public enum PacketType
    {
        Handshake,
        SpawnEntity,
        RemoveEntity,
        Relocation,
        Visibility,
        LobbyState,
        Disconnect,
        Start,
        Loaded,
        AllLoaded,
        Ready,
        Finish,
        AllFinished,
    }

    public interface Packet
    {
        string Dictify();
        PacketType Type();
    }

}
