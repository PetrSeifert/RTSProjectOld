using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Faction))]
public class SelectionController : MonoBehaviour
{
    [Header("Events")] 
    public UnityEvent<RTSFactionEntity[]> onRTSFactionEntitiesSelected;
    public UnityEvent<RTSFactionEntity[]> onRTSFactionEntitiesDeselected;
    public UnityEvent onSelectionCleared;
    
    [HideInInspector] public Dictionary<UnitType, List<Unit>> selectedUnitsByUnitType = new();
    [HideInInspector] public Faction faction;
    [HideInInspector] public List<Unit> selectableUnits = new();
    [HideInInspector] public Building selectedBuilding;


    void Awake()
    {
        faction = GetComponent<Faction>();
    }

    public void HandleUnitSelection(Unit unit)
    {
        if (!selectedUnitsByUnitType.ContainsKey(unit.type))
        {
            selectedUnitsByUnitType.Add(unit.type, new List<Unit>{unit});
            onRTSFactionEntitiesSelected.Invoke(new RTSFactionEntity[]{unit});
        }
        else
        {
            if (selectedUnitsByUnitType[unit.type].Contains(unit))
            {
                if (!InputManager.Instance.shiftHeld) return;
                
                DeselectUnit(unit);
                onRTSFactionEntitiesDeselected.Invoke(new RTSFactionEntity[]{unit});
                
                if (selectedUnitsByUnitType[unit.type].Count == 0) 
                    onSelectionCleared.Invoke(); 
                
                return;
            }
            selectedUnitsByUnitType[unit.type].Add(unit);
            onRTSFactionEntitiesSelected.Invoke(new RTSFactionEntity[]{unit});
        }
        unit.ToggleSelectedColor();
    }

    public void DeselectUnit(Unit unit)
    {
        selectedUnitsByUnitType[unit.type].Remove(unit);
        unit.ToggleSelectedColor();
    }

    public void ClearSelection()
    {
        onSelectionCleared.Invoke();
        selectedBuilding = null;
        foreach (UnitType unitType in selectedUnitsByUnitType.Keys.ToList())
        {
            foreach (Unit unit in selectedUnitsByUnitType[unitType])
            {
                unit.ToggleSelectedColor();
            }
            selectedUnitsByUnitType[unitType].Clear();
        }
    }
}
