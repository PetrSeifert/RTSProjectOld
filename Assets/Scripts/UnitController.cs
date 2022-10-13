using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitController : MonoBehaviour
{
    [SerializeField] Faction faction;
    
    Dictionary<UnitType, List<Unit>> waitingUnitsByUnitType = new();

    void Update()
    {
        if (faction.buildingPlacer.placingBuilding) return;
        
        if (InputManager.Instance.secondaryHeld) DecideAndSetUnitActions();
    }
    
    void DecideAndSetUnitActions()
    {
        foreach (UnitType unitType in faction.selectionController.selectedUnitsByUnitType.Keys)
            waitingUnitsByUnitType[unitType] = new List<Unit>(faction.selectionController.selectedUnitsByUnitType[unitType]); //Used list to deep copy units from dictionary

        Vector3 mouseInWorld = GameManager.Instance.mouseInWorld;
        Vector3 clickDirection = (mouseInWorld - GameManager.Instance.cameraTransform.position).normalized;
        
        bool rayHitUnit = Physics.Raycast(mouseInWorld, clickDirection, out RaycastHit hitInfoUnit, 210, GameManager.Instance.unitLayerMask);
        bool rayHitBuilding = Physics.Raycast(mouseInWorld, clickDirection, out RaycastHit hitInfoBuilding, 210, GameManager.Instance.buildingLayerMask);
        bool rayHitTerrain = Physics.Raycast(mouseInWorld, clickDirection, out RaycastHit hitInfoTerrain, 210, GameManager.Instance.terrainLayerMask);
        bool rayHitResourceSource = Physics.Raycast(mouseInWorld, clickDirection, out RaycastHit hitInfoResourceSource, 210, GameManager.Instance.resourceSourceLayerMask);
        
        foreach (UnitType unitType in waitingUnitsByUnitType.Keys)
        {
            Market hitMarket = rayHitBuilding ? hitInfoBuilding.collider.GetComponent<Market>() : null; //Todo: Change market to building type
            Building hitBuilding = rayHitBuilding ? hitInfoBuilding.collider.GetComponent<Building>() : null;
            foreach (Unit unit in waitingUnitsByUnitType[unitType].ToList())
            {
                
                if (rayHitUnit && hitInfoUnit.collider.GetComponentInParent<Unit>().faction != faction)
                {
                    unit.SetAction(new AttackAction(unit, hitInfoUnit.collider.GetComponentInParent<IDamageable>()));
                    waitingUnitsByUnitType[unitType].Remove(unit);
                }
                else if (rayHitBuilding && hitMarket && unit is ITrading)
                {
                    unit.SetAction(new TradingAction(unit, hitMarket));
                    waitingUnitsByUnitType[unitType].Remove(unit);
                }
                else if (rayHitBuilding && hitBuilding && unit is Villager)
                {
                    unit.SetAction(new BuildingAction(unit, hitBuilding));
                    waitingUnitsByUnitType[unitType].Remove(unit);
                }
                else if (rayHitResourceSource && unit is IGathering)
                {
                    unit.SetAction(new GatheringAction(unit, hitInfoResourceSource.collider.GetComponent<ResourceSource>()));
                    waitingUnitsByUnitType[unitType].Remove(unit);
                }
                else if (rayHitTerrain)
                {
                    unit.SetAction(new MoveToPointAction(unit, hitInfoTerrain.point));
                    waitingUnitsByUnitType[unitType].Remove(unit);
                }
            }
            if (waitingUnitsByUnitType[unitType].Count != 0) Debug.LogError("List isn't clear");
        }
    }
}
