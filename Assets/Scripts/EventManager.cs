using UnityEngine.Events;

public class EventManager : PersistentSingleton<EventManager>
{
    public UnityEvent onResourcesAmountChanged;
    public UnityEvent<RTSFactionEntity[]> onRTSFactionEntitiesSelected;
    public UnityEvent<RTSFactionEntity[]> onRTSFactionEntitiesDeselected;
    public UnityEvent onSelectionCleared;
    public UnityEvent onTerrainGenerated;
    public UnityEvent onObstacleUpdated; //Called when obstacle was spawned or destroyed

    protected override void Awake()
    {
        base.Awake();
        onResourcesAmountChanged = new UnityEvent();
        onRTSFactionEntitiesSelected = new UnityEvent<RTSFactionEntity[]>();
        onRTSFactionEntitiesDeselected = new UnityEvent<RTSFactionEntity[]>();
        onSelectionCleared = new UnityEvent();
        onTerrainGenerated = new UnityEvent();
    }

    void OnDestroy()
    {
        onResourcesAmountChanged.RemoveAllListeners();
        onRTSFactionEntitiesDeselected.RemoveAllListeners();
        onRTSFactionEntitiesSelected.RemoveAllListeners();
        onSelectionCleared.RemoveAllListeners();
        onTerrainGenerated.RemoveAllListeners();
    }
}
