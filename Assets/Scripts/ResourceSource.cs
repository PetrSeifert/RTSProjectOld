using System.Collections.Generic;
using UnityEngine;

public class ResourceSource : MonoBehaviour
{
    public float timeToGather;
    
    [SerializeField] ResourceType resourceType;
    [SerializeField] bool infiniteSource;
    [SerializeField] int remainingGathers;

    public Dictionary<ResourceType, int> GetResource()
    {
        if (infiniteSource) return new Dictionary<ResourceType, int> {{resourceType, 1}};
        remainingGathers--;
        if (remainingGathers == 0) Destroy(gameObject);
        return new Dictionary<ResourceType, int> {{resourceType, 1}};
    }
}
