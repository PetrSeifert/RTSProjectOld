using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum BuildingType
{
    Storage
}

[Serializable] public class BuildingTypeHashSet : SerializableHashSet<BuildingType> {}

public abstract class Building : RTSFactionEntity
{
    public BuildingTypeHashSet buildingTypeSet;
    public float buildTime;
    public bool built;
    [HideInInspector] public bool beingDestroyed;
    
    [Header("Events")]
    public UnityEvent onBuilt;
    public UnityEvent onBuiltDestroy;
    public UnityEvent onDestroy;
    
    KeyValuePair<Villager, int> buildersAmountPair;
    
    protected virtual void Awake()
    {
        BuildingManager.Instance.AddBuilding(faction.owner, this);
        SetBuildingColor();
    }

    protected virtual void Update()
    {
        if (built || !buildersAmountPair.Key) return;
        buildTime -= buildersAmountPair.Key.buildSpeedRate * buildersAmountPair.Value * Time.deltaTime;
        buildersAmountPair = new KeyValuePair<Villager, int>(buildersAmountPair.Key, 0);
        if (buildTime > 0) return;
        onBuilt.Invoke();
        built = true;
        SetBuildingColor();
    }

    public void AddBuilder(Villager villager)
    {
        buildersAmountPair = !buildersAmountPair.Key ? new KeyValuePair<Villager, int>(villager, 1) : new KeyValuePair<Villager, int>(villager, buildersAmountPair.Value + 1);
    }

    void SetBuildingColor()
    {
        renderer.material.color = built ? Color.grey : new Color(1f, 0.62f, 0f);
    }

    public void StartDestroying()
    {
        if (built) onBuiltDestroy.Invoke();
        onDestroy.Invoke();
        beingDestroyed = true;
        //Todo:Play destroy animation and destroy
    }
}
