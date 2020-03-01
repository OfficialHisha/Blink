public class LobbyClient
{
    public string Name { get; }
    public bool Ready { get; set; } = false;

    public LobbyClient(string name)
    {
        Name = name;
    }
}
