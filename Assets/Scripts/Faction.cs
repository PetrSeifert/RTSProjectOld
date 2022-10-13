using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public enum FactionType
{
    Neutral, Green, Blue, Yellow, Red
}

[Serializable] public class FactionTypeHashSet : SerializableHashSet<FactionType> {}

public class Faction : MonoBehaviour
{
    public FactionType owner;
    public Color color;
    public SelectionController selectionController;
    public UnitController unitController;
    public BuildingPlacer buildingPlacer;
    public bool localPlayerControlled;
    public Transform unitsHolder;
    public Transform buildingsHolder;
    public int defaultUnitsAmount;

    [SerializeField] Building mainHallPrefab;
    [SerializeField] Unit villagerPrefab;
    [SerializeField] DictionaryAmountPerResource defaultResources; //Default resources that player receives when game starts
    
    [HideInInspector] public int storageSpace; //Every ResourceType has same amount of storage space
    
    public Dictionary<ResourceType, int> resourceStorage = new();

    void Awake()
    {
        if (localPlayerControlled)
        {
            UI_Manager.Instance.playerFaction = this;
            GameManager.Instance.mainCamera = Camera.main;
            GameManager.Instance.cameraTransform = Camera.main.transform;
        }
    }

    void Start()
    {
        GameManager.Instance.aliveFactionsByFactionType.Add(owner, this);
        Physics.Raycast(transform.position + Vector3.up * 50, Vector3.down, out RaycastHit hitInfo, 150, GameManager.Instance.terrainLayerMask);
        mainHallPrefab.transform.rotation = Quaternion.FromToRotation(transform.up, hitInfo.normal);
        mainHallPrefab.transform.position = new Vector3(hitInfo.point.x, hitInfo.point.y + mainHallPrefab.transform.localScale.y / 2, hitInfo.point.z);
        buildingPlacer.PlaceBuilding(mainHallPrefab.transform, mainHallPrefab);
        SpawnDefaultVillagers();
        foreach (ResourceType resourceType in defaultResources.Keys.ToList())
        {
            if (defaultResources[resourceType] > storageSpace) 
                defaultResources[resourceType] = storageSpace;
            resourceStorage.Add(resourceType, defaultResources[resourceType]);
        }
        EventManager.Instance.onResourcesAmountChanged.Invoke();
    }

    void SpawnDefaultVillagers()
    {
        Vector3 unitSpawnPosition = transform.position + new Vector3(10, 50, 0);
        for (int i = 0; i < defaultUnitsAmount; i++)
        {
            bool validPlacement = false;
            RaycastHit hitInfo = new();
            while (!validPlacement)
            {
                unitSpawnPosition += new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
                Physics.Raycast(unitSpawnPosition, Vector3.down, out hitInfo, 200, GameManager.Instance.noTerrainLayerMask);
                if (hitInfo.collider != null) continue;
                Physics.Raycast(unitSpawnPosition, Vector3.down, out hitInfo, 200, GameManager.Instance.terrainLayerMask);
                if (!(Vector3.Angle(hitInfo.normal, Vector3.up) > 26))
                    validPlacement = true;
            }
            GameObject unitObject = Instantiate(villagerPrefab.myPrefab, unitSpawnPosition, Quaternion.identity, unitsHolder);
            Unit spawnedUnit = unitObject.GetComponent<Unit>();
            spawnedUnit.faction = this;
            spawnedUnit.SetAction(new MoveToPointAction(spawnedUnit, unitSpawnPosition));
        }
    }

    public void StoreResources(Dictionary<ResourceType, int> resources)
    {
        foreach (ResourceType resourceType in resources.Keys.ToList())
        {
            /*if (resourceStorage[resourceType] + resources[resourceType] > storageSpace) 
                resources[resourceType] = storageSpace - resourceStorage[resourceType];*/
            resourceStorage[resourceType] += resources[resourceType];
        }
        EventManager.Instance.onResourcesAmountChanged.Invoke();
    }

    public void UseResources(DictionaryAmountPerResource resources)
    {
        foreach (ResourceType resourceType in resources.Keys)
        {
            resourceStorage[resourceType] -= resources[resourceType];
        }
        EventManager.Instance.onResourcesAmountChanged.Invoke();
    }

    public bool HasEnoughResources(DictionaryAmountPerResource resources)
    {
        foreach (ResourceType resourceType in resources.Keys)
        {
            if (resourceStorage[resourceType] < resources[resourceType]) return false;
        }
        return true;
    }
}
