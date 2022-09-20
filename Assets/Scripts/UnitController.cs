using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitComparerByDistance : IComparer<Unit>
{
    public int Compare(Unit x, Unit y)
    {
        if (x == null || y == null) return 0;
        if (!x.navMeshAgent.enabled || !y.navMeshAgent.enabled) return 0;

        return x.navMeshAgent.remainingDistance.CompareTo(y.navMeshAgent.remainingDistance);
    }
}

public class UnitController : MonoBehaviour
{
    [SerializeField] Faction faction;
    
    Dictionary<UnitType, List<Unit>> waitingUnitsByUnitType = new();
    List<Unit> movingUnits = new();
    UnitComparerByDistance unitComparerByDistance = new();

    void Update()
    {
        if (faction.buildingPlacer.placingBuilding) return;
        
        if (InputManager.Instance.secondaryHeld) SetUnitActions();
    }
    
    void SetUnitActions()
    {
        foreach (UnitType unitType in faction.selectionController.selectedUnitsByUnitType.Keys)
            waitingUnitsByUnitType[unitType] = new List<Unit>(faction.selectionController.selectedUnitsByUnitType[unitType]); //Used list to deep copy units from dictionary

        Vector3 mouseInWorld = GameManager.Instance.mouseInWorld;
        Vector3 clickDirection = (mouseInWorld - GameManager.Instance.cameraTransform.position).normalized;
        
        bool rayHitUnit = Physics.Raycast(mouseInWorld, clickDirection, out RaycastHit hitInfoUnit, 210, GameManager.Instance.unitLayerMask);
        bool rayHitBuilding = Physics.Raycast(mouseInWorld, clickDirection, out RaycastHit hitInfoBuilding, 210, GameManager.Instance.buildingLayerMask);
        bool rayHitTerrain = Physics.Raycast(mouseInWorld, clickDirection, out RaycastHit hitInfoTerrain, 210, GameManager.Instance.terrainLayerMask);
        bool rayHitResourceSource = Physics.Raycast(mouseInWorld, clickDirection, out RaycastHit hitInfoResourceSource, 210, GameManager.Instance.resourceSourceLayerMask);
        
        movingUnits.Clear(); //Throw references to moving units
        foreach (UnitType unitType in waitingUnitsByUnitType.Keys)
        {
            foreach (Unit unit in waitingUnitsByUnitType[unitType].ToList())
            {
                if (rayHitUnit && hitInfoUnit.collider.GetComponent<Unit>().faction != faction)
                {
                    unit.SetAction(new AttackAction(unit, hitInfoUnit.collider.GetComponent<IDamageable>()));
                    waitingUnitsByUnitType[unitType].Remove(unit);
                }
                else if (rayHitBuilding)
                {
                    unit.SetAction(new BuildingAction(unit, hitInfoBuilding.collider.GetComponent<Building>()));
                    waitingUnitsByUnitType[unitType].Remove(unit);
                }
                else if (rayHitResourceSource)
                {
                    unit.SetAction(new GatheringAction(unit, hitInfoResourceSource.collider.GetComponent<ResourceSource>()));
                    waitingUnitsByUnitType[unitType].Remove(unit);
                }
                else if (rayHitTerrain)
                {
                    unit.SetAction(new MoveToPointAction(unit, hitInfoTerrain.point));
                    movingUnits.Add(unit);
                    waitingUnitsByUnitType[unitType].Remove(unit);
                }
            }
            if (waitingUnitsByUnitType[unitType].Count != 0) Debug.LogError("List isn't clear");
        }
    }
    
    public void SetUnitAgentsPriority()
    {
        movingUnits.Sort(0, movingUnits.Count, unitComparerByDistance);
        int priority = 0;
        foreach (Unit movingUnit in movingUnits)
        {
            movingUnit.navMeshAgent.avoidancePriority = priority;            
            priority++;
        }
    }
}
