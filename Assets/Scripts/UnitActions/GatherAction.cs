using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum GatheringState
{
    ReachSource, GatherFromSource, FindNearestStorage, ReachNearestStorage, StoreResources, ReachLastSourcePosition
}

public class GatheringAction : UnitAction
{
    readonly ResourceSource resourceSource;
    readonly IGathering gatheringUnit;
    Vector3 lastResourceSourcePosition;
    Building nearestStorage;
    float gatheringTime;
    Dictionary<GatheringState, Action> functionPerState;
    bool didReachedDestination;

    public GatheringAction(Unit unit, ResourceSource resourceSource) : base(unit)
    {
        gatheringUnit = unit as IGathering;
        gatheringUnit.GatheringState = GatheringState.ReachSource;
        this.resourceSource = resourceSource;
        lastResourceSourcePosition = resourceSource.transform.position;
        functionPerState = new Dictionary<GatheringState, Action>
        {
            {GatheringState.ReachSource, NavReachSource},
            {GatheringState.GatherFromSource, GatherFromSource},
            {GatheringState.FindNearestStorage, FindNearestStorage},
            {GatheringState.ReachNearestStorage, NavReachNearestStorage},
            {GatheringState.StoreResources, StoreResources},
            {GatheringState.ReachLastSourcePosition, NavReachLastSourcePosition}
        };
        targetTransform = resourceSource.gameObject.transform;
    }
    
    public override void Update()
    {
        if (gatheringUnit == null)
        {
            NavReachSource();
            return;
        }
        functionPerState[gatheringUnit.GatheringState]();
    }

    void NavReachSource()
    {
        if (!resourceSource)
        {
            gatheringUnit.GatheringState = GatheringState.ReachLastSourcePosition;
            return;
        }
        unit.NavigateToTarget(resourceSource.transform.position);
        targetTransform = resourceSource.gameObject.transform;
    }
    
    void NavReachLastSourcePosition()
    {
        unit.NavigateToTarget(lastResourceSourcePosition);
        if (!unit.navigationRunning || unit.navMeshAgent.pathPending) return;
        if (unit.navMeshAgent.remainingDistance > unit.navMeshAgent.stoppingDistance) return;
        unit.StopNavigation();
        Deactivate(); //Todo: Try to find new resource source
    }

    void GatherFromSource()
    {
        if (!resourceSource)
        {
            gatheringUnit.GatheringState = GatheringState.FindNearestStorage;
            gatheringTime = 0;
            return;
        }
        gatheringTime += Time.deltaTime;
        if (gatheringTime < resourceSource.timeToGather) return;
        Dictionary<ResourceType, int> gatheredResource = resourceSource.GetResource();
        int deltaToFullInventory = gatheringUnit.UsedInventorySpace - gatheringUnit.InventorySpace;
        if (deltaToFullInventory + gatheredResource.First().Value >= 0)
        {
            gatheringUnit.ResourcesInventory[gatheredResource.First().Key] += -deltaToFullInventory;
            gatheringUnit.UsedInventorySpace += -deltaToFullInventory;
            gatheringUnit.GatheringState = GatheringState.FindNearestStorage;
            gatheringTime = 0;
            return;
        }
        gatheringUnit.ResourcesInventory[gatheredResource.First().Key] += gatheredResource.First().Value;
        gatheringUnit.UsedInventorySpace += gatheredResource.First().Value;
        gatheringTime -= resourceSource.timeToGather;
    }

    void FindNearestStorage()
    {
        nearestStorage = BuildingManager.Instance.GetRequesterNearestBuilding(unit.faction.owner, unit.transform, BuildingType.Storage);
        targetTransform = nearestStorage.gameObject.transform;
        gatheringUnit.GatheringState = GatheringState.ReachNearestStorage;
    }

    void NavReachNearestStorage()
    {
        unit.NavigateToTarget(nearestStorage.transform.position);
    }

    void StoreResources()
    {
        GameManager.Instance.aliveFactionsByFactionType[unit.faction.owner].StoreResources(gatheringUnit.ResourcesInventory);
        gatheringUnit.ClearResourcesInventory();
        gatheringUnit.GatheringState = GatheringState.ReachSource;
    }

    protected override void StopIfCollidedWithTarget(Transform transform)
    {
        if (!targetTransform) return;
        if (transform != targetTransform) return;
        unit.StopNavigation();
        targetTransform = null;
        if (gatheringUnit.GatheringState == GatheringState.ReachSource)
        {
            gatheringUnit.GatheringState = GatheringState.GatherFromSource;
        }
        else if (gatheringUnit.GatheringState == GatheringState.ReachNearestStorage)
        {
            gatheringUnit.GatheringState = GatheringState.StoreResources;
        }
    }
}
