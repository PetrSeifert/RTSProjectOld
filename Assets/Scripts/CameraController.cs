using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] float movementSpeed = 5;
    [SerializeField] int maxOutZoom;
    [SerializeField] int minOutZoom;

    void Update()
    {
        //Moving
        Transform transform1 = transform;
        Vector3 forward = transform1.forward;
        Vector3 right = transform1.right;
        float horizontal = InputManager.Instance.horizontal;
        float vertical = InputManager.Instance.vertical;
        transform1.position += new Vector3(forward.x * vertical + right.x * horizontal, 0, forward.z * vertical + right.z * horizontal).normalized * (movementSpeed * Time.deltaTime);
        
        //Zooming
        float zoomDelta = InputManager.Instance.scrollDelta;
        if (maxOutZoom < transform1.position.y + forward.y * zoomDelta) zoomDelta = transform1.position.y - maxOutZoom;
        else if (minOutZoom > transform1.position.y + forward.y * zoomDelta) zoomDelta = transform1.position.y - minOutZoom;
        transform1.position += forward * zoomDelta;
    }
}
