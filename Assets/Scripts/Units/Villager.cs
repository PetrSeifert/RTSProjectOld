using System.Collections.Generic;
using UnityEngine;

public class Villager : Unit, IGathering
{
    [Header("Building")]
    public float buildSpeedRate;
    [HideInInspector] public BuildingState buildingState;
    
    [Header("Gathering")]
    // Todo: Change inventory space based on profession
    [SerializeField] int inventorySpace;
    [SerializeField] DictionaryAmountPerResource resourcesInventory;
    
    public int InventorySpace { get => inventorySpace;
        set => inventorySpace = value; }
    public Dictionary<ResourceType, int> ResourcesInventory { get; set; } = new();
    public int UsedInventorySpace { get; set; }
    public GatheringState GatheringState { get; set; }

    protected override void Awake()
    {
        base.Awake();
        foreach (ResourceType resourceType in resourcesInventory.Keys)
        {
            ResourcesInventory.Add(resourceType, resourcesInventory[resourceType]);
        }
    }

    public void ClearResourcesInventory()
    {
        foreach (ResourceType resourceType in resourcesInventory.Keys)
        {
            ResourcesInventory[resourceType] = 0;
        }
        UsedInventorySpace = 0;
    }
}
