using UnityEngine;

public class ResetPosition : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        collision.transform.position = new Vector3(0, 0);
        NetworkManager.instance.Send(new RelocationPacket(collision.GetComponent<NetworkEntity>().entityId, 0, 0));
    }
}
