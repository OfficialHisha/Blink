using UnityEngine;

public class Goal : MonoBehaviour
{
    RoomManager roomManager;
    bool hasFinished = false;

    private void Awake()
    {
        roomManager = GameObject.Find("RoomManager").GetComponent<RoomManager>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasFinished)
            return;

        NetworkManager.instance.Send(new FinishPacket(roomManager.GetTimeStamp()));
    }
}
