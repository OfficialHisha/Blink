using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CharacterController2D))]
public class NetworkEntity : MonoBehaviour
{
    public float moveSpeed = 10f;

    [HideInInspector]
    public int entityId;

    [HideInInspector]
    public bool isSelf;

    [HideInInspector]
    public LobbyClient owner;

    Vector3 lastSentPosition;

    CharacterController2D controller;
    float horizontalMove;
    bool jump = false;
    bool canMove = true;

    SpriteRenderer render;
    Rigidbody2D rb;

    public void Setup(int entityId, bool isSelf, LobbyClient owner)
    {
        this.entityId = entityId;
        this.isSelf = isSelf;
        this.owner = owner;

        if (!isSelf)
            return;

        render = GetComponent<SpriteRenderer>();

        rb = GetComponent<Rigidbody2D>();
        rb.WakeUp();

        GetComponent<BoxCollider2D>().enabled = true;

        controller = GetComponent<CharacterController2D>();
        controller.enabled = true;

        Camera.main.GetComponent<SmoothFollow>().target = transform;
    }

    void Update()
    {
        if (!isSelf)
            return;

        horizontalMove = Input.GetAxisRaw("Horizontal") * moveSpeed;

        if (Input.GetButtonDown("Jump"))
        {
            jump = true;
        }

        if (Input.GetButtonDown("Visual"))
        {
            render.enabled = true;
            canMove = false;
            rb.Sleep();
            NetworkManager.instance.Send(new VisibilityPacket(entityId, true));
        }
        if (Input.GetButtonUp("Visual"))
        {
            render.enabled = false;
            canMove = true;
            rb.WakeUp();
            NetworkManager.instance.Send(new VisibilityPacket(entityId, false));
        }

        if (Vector3.Distance(transform.position, lastSentPosition) > .5f)
        {
            NetworkManager.instance.Send(new RelocationPacket(entityId, transform.position));
            lastSentPosition = transform.position;
        }
    }

    void FixedUpdate()
    {
        if (!isSelf)
            return;

        if (!canMove)
            return;

        // Move our character
        controller.Move(horizontalMove * Time.fixedDeltaTime, jump);
        jump = false;
    }
}
