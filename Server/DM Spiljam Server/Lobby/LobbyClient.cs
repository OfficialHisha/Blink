using System.Net.Sockets;

namespace DM_Spiljam_Server
{
    class LobbyClient
    {
        public string Name { get; }
        public bool Ready { get; set; } = false;

        public LobbyClient(string name)
        {
            Name = name;
        }
    }
}
