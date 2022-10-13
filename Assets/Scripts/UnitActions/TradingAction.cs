using System;
using System.Collections.Generic;
using UnityEngine;

public enum TradingState
{
    ReachMarket, FindNearestStorage, ReachNearestStorage, StoreResources
}

public class TradingAction : UnitAction
{
    readonly Market market;
    readonly ITrading tradingUnit;
    Building nearestStorage;
    Dictionary<TradingState, Action> functionPerState;
    bool didReachedDestination;
    Vector3 lastPosition;

    public TradingAction(Unit unit, Market market) : base(unit)
    {
        tradingUnit = unit as ITrading;
        tradingUnit.TradingState = TradingState.ReachMarket;
        tradingUnit.SqrTraveledDistance = 0;
        this.market = market;
        lastPosition = unit.transform.position;
        functionPerState = new Dictionary<TradingState, Action>
        {
            {TradingState.ReachMarket, NavReachMarket},
            {TradingState.FindNearestStorage, FindNearestStorage},
            {TradingState.ReachNearestStorage, NavReachNearestStorage},
            {TradingState.StoreResources, StoreResources}
        };
        targetTransform = market.gameObject.transform;
    }
    
    public override void Update()
    {
        Debug.Log("Trader action");
        functionPerState[tradingUnit.TradingState]();
    }

    void NavReachMarket()
    {
        if (unit.richAI.destination != market.transform.position) unit.NavigateToTarget(market.transform.position, market.GetComponent<MeshFilter>().mesh.bounds.extents.x * market.transform.lossyScale.x * 1.4f + unit.GetComponentInChildren<MeshFilter>().mesh.bounds.extents.x);
        targetTransform = market.gameObject.transform;
        if (tradingUnit == null) return;
        tradingUnit.SqrTraveledDistance += Vector3.SqrMagnitude(lastPosition - unit.transform.position);
        if (unit.richAI.pathPending) return;
        if (!(unit.richAI.reachedEndOfPath)) return;
        tradingUnit.CollectResources();
        tradingUnit.TradingState = TradingState.FindNearestStorage;
        targetTransform = null;
    }

    void FindNearestStorage()
    {
        nearestStorage = BuildingManager.Instance.GetRequesterNearestBuilding(unit.faction.owner, unit.transform, BuildingType.Storage);
        targetTransform = nearestStorage.gameObject.transform;
        tradingUnit.TradingState = TradingState.ReachNearestStorage;
    }

    void NavReachNearestStorage()
    {
        if (unit.richAI.destination != nearestStorage.transform.position) unit.NavigateToTarget(nearestStorage.transform.position, nearestStorage.GetComponent<MeshFilter>().mesh.bounds.extents.x * nearestStorage.transform.lossyScale.x * 1.4f + unit.GetComponentInChildren<MeshFilter>().mesh.bounds.extents.x);
        if (unit.richAI.pathPending) return;
        if (!unit.richAI.reachedEndOfPath) return;
        tradingUnit.TradingState = TradingState.StoreResources;
        targetTransform = null;
    }

    void StoreResources()
    {
        GameManager.Instance.aliveFactionsByFactionType[unit.faction.owner].StoreResources(tradingUnit.ResourcesInventory);
        tradingUnit.SqrTraveledDistance = 0;
        tradingUnit.ClearResourcesInventory();
        tradingUnit.TradingState = TradingState.ReachMarket;
    }
}
