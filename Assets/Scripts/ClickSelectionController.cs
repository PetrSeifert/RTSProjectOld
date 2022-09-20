using UnityEngine;

[RequireComponent(typeof(SelectionController))]
public class ClickSelectionController : MonoBehaviour
{
    [SerializeField] float dragDelay;

    SelectionController selectionController;
    int clickSelectionLayerMask;
    float remainingDragDelay;
    
    void Awake()
    {
        selectionController = GetComponent<SelectionController>();
        clickSelectionLayerMask =~ LayerMask.GetMask("Terrain");
    }

    void Update()
    {
        if (InputManager.Instance.primaryHeld)
        {
            remainingDragDelay = dragDelay - InputManager.Instance.primaryHoldTime;
        }
        else if (InputManager.Instance.primaryUp && remainingDragDelay > 0)
        { 
            ClickSelection();
        }
    }

    void ClickSelection()
    {
        if (UI_Manager.Instance.ClickedOnUI()) return;
        bool hitSelectable = Physics.Raycast(GameManager.Instance.mouseCameraRay, out RaycastHit hitInfo, 210, clickSelectionLayerMask);
        if (!hitSelectable && !InputManager.Instance.shiftHeld)
        {
            selectionController.ClearSelection();
            return;
        }

        RTSFactionEntity hitFactionEntity = hitInfo.collider.GetComponent<RTSFactionEntity>();
        if (hitFactionEntity.faction != selectionController.faction) return;
        Unit unit = hitFactionEntity as Unit;
        if (unit)
        {
            selectionController.onRTSFactionEntitiesDeselected.Invoke(new RTSFactionEntity[]{selectionController.selectedBuilding});
            selectionController.selectedBuilding = null; //We dont want to have units and building selected together
            if (!InputManager.Instance.shiftHeld)
                selectionController.ClearSelection();
            
            selectionController.HandleUnitSelection(unit);
            return;
        }
        
        Building building = hitFactionEntity as Building;
        if (building)
        {
            selectionController.ClearSelection(); //We want to have only one building selected and nothing else
            selectionController.onRTSFactionEntitiesSelected.Invoke(new RTSFactionEntity[]{building});
            selectionController.selectedBuilding = building;
        }
    }
    
}
