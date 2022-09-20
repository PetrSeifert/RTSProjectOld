using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public enum FactionType
{
    Green, Blue, Yellow, Red
}

[Serializable] public class FactionTypeHashSet : SerializableHashSet<FactionType> {}

public class Faction : MonoBehaviour
{
    public FactionType owner;
    public SelectionController selectionController;
    public UnitController unitController;
    public BuildingPlacer buildingPlacer;
    public UnityEvent onResourcesChanged;
    public bool localPlayerControlled;
    public Transform unitsHolder;
    public Transform buildingsHolder;
    
    [SerializeField] DictionaryAmountPerResource defaultResources; //Default resources that player receives when game starts
    
    [HideInInspector] public int storageSpace; //Every ResourceType has same amount of storage space
    
    public Dictionary<ResourceType, int> resourceStorage = new();

    void Awake()
    {
        GameManager.Instance.aliveFactionsByFactionType.Add(owner, this);
    }

    void Start()
    {
        foreach (ResourceType resourceType in defaultResources.Keys.ToList())
        {
            if (defaultResources[resourceType] > storageSpace) 
                defaultResources[resourceType] = storageSpace;
            resourceStorage.Add(resourceType, defaultResources[resourceType]);
        }
        onResourcesChanged.Invoke();
    }

    public void StoreResources(Dictionary<ResourceType, int> resources)
    {
        foreach (ResourceType resourceType in resources.Keys)
        {
            if (defaultResources[resourceType] > storageSpace) 
                defaultResources[resourceType] = storageSpace;
            resourceStorage[resourceType] += resources[resourceType];
        }
        onResourcesChanged.Invoke();
    }

    public void UseResources(DictionaryAmountPerResource resources)
    {
        foreach (ResourceType resourceType in resources.Keys)
        {
            resourceStorage[resourceType] -= resources[resourceType];
        }
        onResourcesChanged.Invoke();
    }

    public bool HasEnoughResources(DictionaryAmountPerResource resources)
    {
        foreach (ResourceType resourceType in resources.Keys)
        {
            if (resourceStorage[resourceType] < resources[resourceType]) return false;
        }
        return true;
    }
}
