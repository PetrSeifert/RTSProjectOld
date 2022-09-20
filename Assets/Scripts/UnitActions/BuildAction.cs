using System;
using System.Collections.Generic;
using UnityEngine;

public enum BuildingState
{
    ReachBuilding, BuildBuilding
}

public class BuildingAction : UnitAction
{
    Villager villager; //Todo: Consider interface
    Building building;
    Dictionary<BuildingState, Action> functionPerState;

    public BuildingAction(Unit unit, Building building) : base(unit)
    {
        villager = unit as Villager;
        this.building = building;
        functionPerState = new Dictionary<BuildingState, Action>
        {
            {BuildingState.ReachBuilding, NavReachBuilding},
            {BuildingState.BuildBuilding, BuildBuilding},
        };
        if (villager) villager.buildingState = BuildingState.ReachBuilding;
        targetTransform = building.gameObject.transform;
    }
    
    public override void Update()
    {
        if (!villager) NavReachBuilding();
        functionPerState[villager.buildingState]();
    }

    void NavReachBuilding()
    {
        if (building.built)
        {
            unit.StopNavigation();
            Deactivate();
            return;
        }
        unit.NavigateToTarget(building.transform.position);
    }

    void BuildBuilding()
    {
        building.AddBuilder(villager);
    }
    
    protected override void StopIfCollidedWithTarget(Transform transform)
    {
        if (!targetTransform) return;
        if (transform != targetTransform) return;
        unit.StopNavigation();
        targetTransform = null;
        if (!villager)
        {
            Deactivate();
            return;
        }
        
        villager.buildingState = BuildingState.BuildBuilding;
    }
}
