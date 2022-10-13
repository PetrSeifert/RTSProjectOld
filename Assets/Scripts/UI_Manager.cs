using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class UI_Manager : Singleton<UI_Manager>
{
    public List<UI_ButtonWithRTSEntity> buttonsWithRtsEntity;
    
    public Faction playerFaction;
    
    [SerializeField] GameObject entityPanel;
    [SerializeField] GameObject villagerUI;
    [SerializeField] GameObject archerUI;
    [SerializeField] GameObject soldierUI;
    [SerializeField] GameObject traderUI;
    [SerializeField] GameObject mainHallUI;
    [SerializeField] GameObject barracksUI;
    [SerializeField] GameObject storageUI;
    [SerializeField] TMP_Text resourcesText;
    [SerializeField] EventSystem eventSystem;

    Dictionary<UnitType, int> numberOfUnitsPerUnitType;

    GraphicRaycaster raycaster;
    PointerEventData pointerEventData;

    protected override void Awake()
    {
        base.Awake();
        raycaster = GetComponent<GraphicRaycaster>();
        
    }

    void Start()
    {
        EventManager.Instance.onRTSFactionEntitiesSelected.AddListener(AddEntitiesToSelectionUIAndUpdate);
        EventManager.Instance.onRTSFactionEntitiesDeselected.AddListener(RemoveEntitiesFromSelectionUIAndUpdate);
        EventManager.Instance.onSelectionCleared.AddListener(HidePanels);
        EventManager.Instance.onResourcesAmountChanged.AddListener(SetIfButtonsUsable);
        EventManager.Instance.onResourcesAmountChanged.AddListener(UpdateResourcesUI);
        HidePanels();
    }

    public bool ClickedOnUI()
    {
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = InputManager.Instance.mousePosition;
        List<RaycastResult> results = new();
        raycaster.Raycast(pointerEventData, results);
        return results.Count > 0;
    }

    void ActivateVillagerUI()
    {
        entityPanel.SetActive(true);
        villagerUI.SetActive(true);
    }
    
    void ActivateSoldierUI()
    {
        entityPanel.SetActive(true);
        soldierUI.SetActive(true);
    }
    
    void ActivateArcherUI()
    {
        entityPanel.SetActive(true);
        archerUI.SetActive(true);
    }
    
    void ActivateTraderUI()
    {
        entityPanel.SetActive(true);
        traderUI.SetActive(true);
    }
    
    void ActivateUnitSpawnerPanel()
    {        
        entityPanel.SetActive(true);
        if (playerFaction.selectionController.selectedBuilding is Baracks)
        {
            barracksUI.SetActive(true);
        }
        else if (playerFaction.selectionController.selectedBuilding is MainHall)
        {
            mainHallUI.SetActive(true);
        }
    }

    void ActivateStoragePanel()
    {
        entityPanel.SetActive(true);
        storageUI.SetActive(true);
    }

    void AddEntitiesToSelectionUIAndUpdate(RTSFactionEntity[] entitiesToAdd)
    {
        foreach (RTSFactionEntity rtsFactionEntity in entitiesToAdd)
        {
            Unit unitToAdd = rtsFactionEntity as Unit;
            if (unitToAdd)
            {
                switch (unitToAdd.type)
                {
                    case UnitType.Villager:
                        ActivateVillagerUI();
                        break;
                    case UnitType.Soldier:
                        ActivateSoldierUI();
                        break;
                    case UnitType.Archer:
                        ActivateArcherUI();
                        break;
                    case UnitType.Trader:
                        ActivateTraderUI();
                        break;
                }
            }
            else
            {
                if (rtsFactionEntity is Storage) ActivateStoragePanel();
                else ActivateUnitSpawnerPanel();
            }
        }
    }
    
    void RemoveEntitiesFromSelectionUIAndUpdate(RTSFactionEntity[] entitiesToRemove)
    {
        foreach (RTSFactionEntity rtsFactionEntity in entitiesToRemove)
        {
            Building buildingToRemove = rtsFactionEntity as Building;
            if (buildingToRemove) HidePanels();
        }
    }

    public void UnitSpawnerButtonClicked(Unit unitToSpawn)
    {
        ISpawnUnits unitSpawner = playerFaction.selectionController.selectedBuilding as ISpawnUnits;
        unitSpawner.AddUnitToQueue(unitToSpawn);
    }

    void HidePanels()
    {
        entityPanel.SetActive(false);
        villagerUI.SetActive(false);
        mainHallUI.SetActive(false);
        barracksUI.SetActive(false);
        archerUI.SetActive(false);
        soldierUI.SetActive(false);
        traderUI.SetActive(false);
        storageUI.SetActive(false);
    }

    void UpdateResourcesUI()
    {
        resourcesText.text =
            $"Money: {playerFaction.resourceStorage[ResourceType.Money]} Food: {playerFaction.resourceStorage[ResourceType.Food]} Wood: {playerFaction.resourceStorage[ResourceType.Wood]} Stone: {playerFaction.resourceStorage[ResourceType.Stone]}";
    }

    void SetIfButtonsUsable()
    {
        foreach (UI_ButtonWithRTSEntity buttonWithRtsEntity in buttonsWithRtsEntity)
        {
            buttonWithRtsEntity.button.interactable = playerFaction.HasEnoughResources(buttonWithRtsEntity.rtsFactionEntity.resourcesNeededToCreateMe);
        }
    }

    public void TogglePlacingBuilding(Building building)
    {
        playerFaction.buildingPlacer.TogglePlacingBuilding(building);
    }
}