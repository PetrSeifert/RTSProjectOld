using System.Collections.Generic;
using UnityEngine;

public class GameManager : PersistentSingleton<GameManager>
{
    public Dictionary<FactionType, Faction> aliveFactionsByFactionType = new(); //All factions alive in current game
        
    public Camera mainCamera;
    public Transform cameraTransform;
    public Vector3 mouseInWorld;
    public Ray mouseCameraRay;
    
    public int terrainLayerMask;
    public int resourceSourceLayerMask;
    public int buildingLayerMask;
    public int unitLayerMask;
    public int noTerrainLayerMask;
    public bool localPlayerWin;
    public bool localPlayerLose;
    public HashSet<ResourceType> resourceTypes;
    public HashSet<UnitType> unitTypes;
    public int mapSize;

    protected override void Awake()
    {
        base.Awake();
        resourceTypes = new HashSet<ResourceType>()
            {ResourceType.Wood, ResourceType.Food, ResourceType.Money,  ResourceType.Stone};
        unitTypes = new HashSet<UnitType>()
            {UnitType.Villager, UnitType.Soldier, UnitType.Archer, UnitType.Trader};
        terrainLayerMask = LayerMask.GetMask("Terrain");
        resourceSourceLayerMask = LayerMask.GetMask("ResourceSource");
        buildingLayerMask = LayerMask.GetMask("Building");
        unitLayerMask = LayerMask.GetMask("Unit");
        noTerrainLayerMask = ~ LayerMask.GetMask("Terrain");
        mapSize = 400;
        Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        mouseCameraRay = mainCamera.ScreenPointToRay(InputManager.Instance.mousePosition);
        mouseInWorld = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));
    }

    public void DestroyFaction(Faction faction) //Todo: Add support for multiplayer
    {
        localPlayerWin = true;
        localPlayerLose = true;
        aliveFactionsByFactionType.Remove(faction.owner);
        foreach (FactionType factionType in aliveFactionsByFactionType.Keys)
        {
            if (!aliveFactionsByFactionType[factionType].localPlayerControlled) localPlayerWin = false;
            else if (aliveFactionsByFactionType[factionType].localPlayerControlled) localPlayerLose = false;
        }
        if (localPlayerLose || localPlayerWin) GameOver();
    }

    void GameOver()
    {
        //Todo: Add gameOver
    }
}
