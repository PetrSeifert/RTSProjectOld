using System.Collections.Generic;
using UnityEngine;

public class ResourceSource : MonoBehaviour
{
    public float timeToGather;
    
    public ResourceType resourceType;
    public int maxGroupSize;
    [SerializeField] bool infiniteSource;
    [SerializeField] int remainingGathers;

    public Dictionary<ResourceType, int> GetResource()
    {
        if (infiniteSource) return new Dictionary<ResourceType, int> {{resourceType, 1}};
        remainingGathers--;
        if (remainingGathers == 0) CustomDestroy();
        return new Dictionary<ResourceType, int> {{resourceType, 1}};
    }

    void CustomDestroy()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Bounds bounds = new(transform.position ,mesh.bounds.size);
        mesh.Clear();
        AstarPath.active.UpdateGraphs(bounds);
        Destroy(gameObject);
    }
}
