﻿using UnityEngine;
using TMPro;
using System.Collections;

public class MenuManager : MonoBehaviour
{
    public TMP_InputField playerNameInput;
    public TMP_InputField serverHostInput;

    Coroutine connRoutine;

    // Start is called before the first frame update
    void Start()
    {
        string playerName = PlayerPrefs.GetString("playerName");
        string serverHost = PlayerPrefs.GetString("lastServerHost");
        if (string.IsNullOrWhiteSpace(playerName))
            playerName = "New Player";
        if (string.IsNullOrWhiteSpace(serverHost))
            serverHost = "localhost";

        playerNameInput.text = playerName;
        serverHostInput.text = serverHost;
    }

    public void OnPlayerNameChanged()
    {
        PlayerPrefs.SetString("playerName", playerNameInput.text);
    }

    public void OnServerHostNameChanged()
    {
        PlayerPrefs.SetString("lastServerHost", serverHostInput.text);
    }

    public void OnConnectButton()
    {
        connRoutine = StartCoroutine(Connect());
    }

    IEnumerator Connect()
    {
        int timeout = 10;
        int current = 0;

        NetworkManager.instance.Connect(serverHostInput.text);

        while (!NetworkManager.instance.connected)
        {
            yield return new WaitForSeconds(1);

            current++;

            if (current == timeout)
                StopCoroutine(connRoutine);
        }

        NetworkManager.instance.Send(new HandshakePacket(playerNameInput.text));

    }

    public void OnQuitButton()
    {
        Application.Quit();
    }
}
