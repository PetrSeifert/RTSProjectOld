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
    ResourceSource resourceSource;
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
        targetTransform = resourceSource.transform;
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
        if (unit.richAI.destination != resourceSource.transform.position) unit.NavigateToTarget(resourceSource.transform.position, resourceSource.GetComponent<MeshFilter>().mesh.bounds.extents.x + unit.GetComponentInChildren<MeshFilter>().mesh.bounds.extents.x);
        targetTransform = resourceSource.gameObject.transform;
        if (unit.richAI.pathPending) return;
        if (!unit.richAI.reachedEndOfPath) return;
        gatheringUnit.GatheringState = GatheringState.GatherFromSource;
        targetTransform = null;
    }
    
    void NavReachLastSourcePosition()
    {
        if (unit.richAI.destination != lastResourceSourcePosition) unit.NavigateToTarget(lastResourceSourcePosition, 0.1f);
        if (unit.richAI.pathPending) return;
        if (!unit.richAI.reachedEndOfPath) return;
        resourceSource = FindNewSourceInRange();
        if (!resourceSource) return;
        gatheringUnit.GatheringState = GatheringState.ReachSource;
        targetTransform = resourceSource.transform;
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
        if (unit.richAI.destination != nearestStorage.transform.position) unit.NavigateToTarget(nearestStorage.transform.position, nearestStorage.GetComponent<MeshFilter>().mesh.bounds.extents.x * nearestStorage.transform.lossyScale.x * 1.4f + unit.GetComponentInChildren<MeshFilter>().mesh.bounds.extents.x);
        if (unit.richAI.pathPending) return;
        if (!unit.richAI.reachedEndOfPath) return;
        gatheringUnit.GatheringState = GatheringState.StoreResources;
        targetTransform = null;
    }

    void StoreResources()
    {
        GameManager.Instance.aliveFactionsByFactionType[unit.faction.owner].StoreResources(gatheringUnit.ResourcesInventory);
        gatheringUnit.ClearResourcesInventory();
        gatheringUnit.GatheringState = GatheringState.ReachSource;
    }

    ResourceSource FindNewSourceInRange()
    {
        Collider[] result = new Collider[1];
        Physics.OverlapBoxNonAlloc(unit.transform.position, new Vector3(5, 10, 5), result, Quaternion.identity,
            GameManager.Instance.resourceSourceLayerMask);
        return result[0] == null ? null : result[0].GetComponent<ResourceSource>();
    }
}