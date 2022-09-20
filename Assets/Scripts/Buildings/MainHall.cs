using System.Collections.Generic;
using UnityEngine;

public class MainHall : Building, IStoring, ISpawnUnits
{
    [Header("Storage")]
    [SerializeField] int storageSpace;
    
    
    Queue<Unit> unitsToSpawnQueue = new();
    float currentProduceTime;
    Vector3 defaultReachPoint;

    public int StorageSpace { get; set; }

    protected override void Awake()
    {
        base.Awake();
        StorageSpace = storageSpace;
        AddStorageSpaceToFaction();
        onBuiltDestroy.AddListener(Destroy);
        defaultReachPoint = transform.position + transform.localScale + Vector3.right * 3;
    }

    protected override void Update()
    {
        base.Update();
        if (!built) return;
        HandleUnitSpawning();
    }

    void OnDestroy()
    {
        onBuiltDestroy.RemoveListener(Destroy);
    }
    
    void HandleUnitSpawning()
    {
        if (unitsToSpawnQueue.Count == 0)
        {
            currentProduceTime = 0;
            return;
        }
        currentProduceTime += Time.deltaTime;
        if (currentProduceTime < unitsToSpawnQueue.Peek().produceTime) return;
        Unit unitToSpawn = unitsToSpawnQueue.Dequeue();
        GameObject unitObject = Instantiate(unitToSpawn.myPrefab, transform.position, Quaternion.identity, faction.unitsHolder);
        unitToSpawn = unitObject.GetComponent<Unit>(); //Update to scene reference
        unitToSpawn.faction = faction;
        unitToSpawn.SetAction(new MoveToPointAction(unitToSpawn, defaultReachPoint));
        currentProduceTime -= unitToSpawn.produceTime;
    }
    
    public void AddUnitToQueue(Unit unit)
    {
        faction.UseResources(unit.resourcesNeededToCreateMe);
        unitsToSpawnQueue.Enqueue(unit);
    }

    public void AddStorageSpaceToFaction()
    {
        faction.storageSpace += StorageSpace;
    }

    public void RemoveStorageSpaceFromFaction()
    {
        faction.storageSpace -= StorageSpace;
    }

    void Destroy()
    {
        GameManager.Instance.DestroyFaction(faction);
    }
}
