using UnityEngine;

public class Market : MonoBehaviour
{
    Mesh mesh;

    void Awake()
    {
        mesh = GetComponent<MeshFilter>().mesh;
    }

    void Start()
    {
        Physics.Raycast(transform.position + Vector3.up * 100, Vector3.down, out RaycastHit hitInfo, 150, GameManager.Instance.terrainLayerMask);
        transform.rotation = Quaternion.FromToRotation(transform.up, hitInfo.normal);
        transform.position = new Vector3(hitInfo.point.x, hitInfo.point.y + mesh.bounds.size.y * 0.5f * transform.lossyScale.y, hitInfo.point.z);
    }
}
