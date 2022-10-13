using System.Collections.Generic;

public class Trader : Unit, ITrading
{
    public ResourceType CurrentResourceTrading { get; set; }
    public TradingState TradingState { get; set; }
    public Dictionary<ResourceType, int> ResourcesInventory { get; set; } = new();
    public float SqrTraveledDistance { get; set; }

    readonly float resourcesPerSqrDistanceUnit = 0.00001f;

    protected override void Awake()
    {
        base.Awake();
        foreach (ResourceType resourceType in GameManager.Instance.resourceTypes)
        {
            ResourcesInventory.Add(resourceType, 0);
        }
    }

    public void CollectResources()
    {
        ResourcesInventory[CurrentResourceTrading] += (int)(SqrTraveledDistance * resourcesPerSqrDistanceUnit);
    }

    public void ClearResourcesInventory()
    {
        foreach (ResourceType resourceType in GameManager.Instance.resourceTypes)
        {
            ResourcesInventory[resourceType] = 0;
        }
    }
}
