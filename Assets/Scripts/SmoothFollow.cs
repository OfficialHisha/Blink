using UnityEngine;

public class SmoothFollow : MonoBehaviour
{
	public Transform target;
	public float damping = 2.0f;
	public Vector2 roomBounds;

	Camera cam;

	private void Awake()
	{
		cam = GetComponent<Camera>();
	}

	void LateUpdate()
	{
		// Early out if we don't have a target
		if (!target) return;

		float wantedY = target.position.y;
		float wantedX = Mathf.Clamp(target.position.x, roomBounds.x, roomBounds.y);

		float currentY = transform.position.y;
		float currentX = transform.position.x;

		// Damp the X
		if (Mathf.Abs(wantedX - currentX) > cam.orthographicSize - 1)
		{
			currentX = Mathf.Lerp(currentX, wantedX, damping * Time.deltaTime);
		}
		// Damp the Y
		if (Mathf.Abs(wantedY - currentY) > cam.orthographicSize - 1)
		{
			currentY = Mathf.Lerp(currentY, wantedY, damping * Time.deltaTime);
		}

		// Set the new position of the camera
		transform.position = new Vector3(currentX, currentY, transform.position.z);
	}
}
