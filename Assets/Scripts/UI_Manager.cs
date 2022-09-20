using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class UI_Manager : Singleton<UI_Manager>
{
    public List<UI_ButtonWithRTSEntity> buttonsWithRtsEntity;
    
    [SerializeField] Faction playerFaction;
    
    [SerializeField] GameObject villagerPanel;
    [SerializeField] GameObject unitSpawnerPanel;
    [SerializeField] TMP_Text resourcesText;
    [SerializeField] EventSystem eventSystem;

    Dictionary<UnitType, int> numberOfUnitsPerUnitType;

    GraphicRaycaster raycaster;
    PointerEventData pointerEventData;

    protected override void Awake()
    {
        base.Awake();
        raycaster = GetComponent<GraphicRaycaster>();
        playerFaction.selectionController.onRTSFactionEntitiesSelected.AddListener(AddEntitiesToSelectionUIAndUpdate);
        playerFaction.selectionController.onRTSFactionEntitiesDeselected.AddListener(RemoveEntitiesFromSelectionUIAndUpdate);
        playerFaction.selectionController.onSelectionCleared.AddListener(HidePanels);
        playerFaction.onResourcesChanged.AddListener(SetIfButtonsUsable);
        playerFaction.onResourcesChanged.AddListener(UpdateResourcesUI);
        HidePanels();
    }

    protected override void OnDestroy()
    {
        playerFaction.onResourcesChanged.RemoveListener(SetIfButtonsUsable);
        playerFaction.onResourcesChanged.RemoveListener(UpdateResourcesUI);
        playerFaction.selectionController.onRTSFactionEntitiesSelected.RemoveListener(AddEntitiesToSelectionUIAndUpdate);
        playerFaction.selectionController.onRTSFactionEntitiesDeselected.RemoveListener(RemoveEntitiesFromSelectionUIAndUpdate);
        playerFaction.selectionController.onSelectionCleared.RemoveListener(HidePanels);
    }

    public bool ClickedOnUI()
    {
        pointerEventData = new PointerEventData(eventSystem);
        pointerEventData.position = InputManager.Instance.mousePosition;
        List<RaycastResult> results = new();
        raycaster.Raycast(pointerEventData, results);
        return results.Count > 0;
    }

    void ActivateVillagerPanel()
    {
        villagerPanel.SetActive(true);
    }
    
    void ActivateUnitSpawnerPanel()
    {
        unitSpawnerPanel.SetActive(true);
    }

    void AddEntitiesToSelectionUIAndUpdate(RTSFactionEntity[] entitiesToAdd)
    {
        foreach (RTSFactionEntity rtsFactionEntity in entitiesToAdd)
        {
            Unit unitToAdd = rtsFactionEntity as Unit;
            if (unitToAdd)
            {
                ActivateVillagerPanel();
            }
            else
            {
                ActivateUnitSpawnerPanel();
            }
        }
    }
    
    void RemoveEntitiesFromSelectionUIAndUpdate(RTSFactionEntity[] entitiesToRemove)
    {
        foreach (RTSFactionEntity rtsFactionEntity in entitiesToRemove)
        {
            Building buildingToRemove = rtsFactionEntity as Building;
            if (buildingToRemove) HideUnitSpawnerPanel();
        }
    }

    public void UnitSpawnerButtonClicked(Unit unitToSpawn)
    {
        ISpawnUnits unitSpawner = playerFaction.selectionController.selectedBuilding as ISpawnUnits;
        unitSpawner.AddUnitToQueue(unitToSpawn);
    }

    void HidePanels()
    {
        villagerPanel.SetActive(false);
        unitSpawnerPanel.SetActive(false);
    }

    void HideUnitSpawnerPanel()
    {
        unitSpawnerPanel.SetActive(false);
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
}