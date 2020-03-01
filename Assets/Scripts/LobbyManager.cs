using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LobbyManager : MonoBehaviour
{
    public GameObject playerListContent;
    public GameObject playerListPrefab;
    public Button readyButton;

    List<GameObject> lobbyPlayers = new List<GameObject>();

    bool ready = false;

    void Start()
    {
        StartCoroutine(UpdatePlayerList());
    }

    IEnumerator UpdatePlayerList()
    {
        while(true)
        {
            foreach (GameObject lobbyPlayer in lobbyPlayers)
            {
                Destroy(lobbyPlayer);
            }
            lobbyPlayers.Clear();
            foreach (LobbyClient client in NetworkManager.instance.lobbyClients)
            {
                GameObject obj = Instantiate(playerListPrefab, playerListContent.transform);
                obj.GetComponent<TextMeshProUGUI>().text = $"{client.Name} - {(client.Ready ? "Ready" : "Not Ready")}";
                lobbyPlayers.Add(obj);
            }

            yield return new WaitForSeconds(1);
        }
    }

    public void OnReadyButton()
    {
        ready = !ready;
        readyButton.GetComponent<Image>().color = ready ? Color.green : Color.white;
        NetworkManager.instance.Send(new ReadyPacket(ready));
    }

    public void OnDisconnectButton()
    {
        NetworkManager.instance.Disconnect();
    }
}
