using System.Collections.Generic;

public interface ITrading
{
    TradingState TradingState { get; set; }
    float SqrTraveledDistance { get; set; }
    public Dictionary<ResourceType, int> ResourcesInventory { get; set; }

    void CollectResources();

    void ClearResourcesInventory();
}
