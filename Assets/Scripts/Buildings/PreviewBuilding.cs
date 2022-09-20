using UnityEngine;

public class PreviewBuilding : MonoBehaviour
{
    public bool canBePlaced;

    Renderer renderer;

    void Awake()
    {
        renderer = GetComponent<Renderer>();
    }

    void OnTriggerEnter(Collider other)
    {
        canBePlaced = false;
        renderer.material.color = Color.red;
    }

    void OnTriggerExit(Collider other)
    {
        canBePlaced = true;
        renderer.material.color = Color.white;
    }
}
