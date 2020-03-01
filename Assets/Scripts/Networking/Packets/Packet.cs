public enum PacketType
{
    Handshake,
    SpawnEntity,
    RemoveEntity,
    Relocation,
    Visibility,
    LobbyState,
    LobbyReturn,
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
