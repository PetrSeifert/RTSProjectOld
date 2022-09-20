using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    public bool placingBuilding;
    
    [SerializeField] Faction faction;
    [SerializeField] GameObject previewBuildingObject;

    PreviewBuilding previewBuilding;
    Building buildingToPlace;

    void Awake()
    {
        previewBuildingObject = Instantiate(previewBuildingObject);
        previewBuilding = previewBuildingObject.GetComponent<PreviewBuilding>();
        previewBuildingObject.SetActive(false);
    }

    void Update()
    {
        if (placingBuilding) HandlePlacingBuilding();
    }

    void HandlePlacingBuilding()
    {
        Vector3 mouseInWorld = GameManager.Instance.mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 1));
        Vector3 clickDirection = (mouseInWorld - GameManager.Instance.cameraTransform.position).normalized;
        bool terrainHit = Physics.Raycast(mouseInWorld, clickDirection, out RaycastHit hitInfo, 100, GameManager.Instance.terrainLayerMask);
        if (!terrainHit)
        {
            Debug.LogError("Did not hit terrain");
            return;
        }
        Vector3 positionForBuilding = new(hitInfo.point.x, hitInfo.point.y + 0.5f, hitInfo.point.z);
        ActivatePreviewBuilding(positionForBuilding);
        if (InputManager.Instance.primaryDown && CanPlaceBuilding())
        {
            if (UI_Manager.Instance.ClickedOnUI()) return;
            faction.UseResources(buildingToPlace.resourcesNeededToCreateMe);
            Building building = PlaceBuilding(positionForBuilding, buildingToPlace);
            foreach (Unit unit in faction.selectionController.selectedUnitsByUnitType[UnitType.Villager])
            {
                unit.SetAction(new BuildingAction(unit, building));
            }
            placingBuilding = false;
        }
    }

    void ActivatePreviewBuilding(Vector3 position)
    {
        previewBuildingObject.SetActive(true);
        previewBuildingObject.transform.position = position;
    }

    Building PlaceBuilding(Vector3 position, Building building)
    {
        previewBuildingObject.SetActive(true);
        previewBuildingObject.transform.position = position;
        building.faction = faction;
        building = Instantiate(building.gameObject, position, Quaternion.identity, faction.buildingsHolder).GetComponent<Building>();
        previewBuildingObject.SetActive(false);
        return building;
    }

    bool CanPlaceBuilding() => previewBuilding.canBePlaced;

    public void TogglePlacingBuilding(Building building)
    {
        if (placingBuilding)
        {
            if (buildingToPlace == building)
            {
                placingBuilding = false;
                previewBuildingObject.SetActive(false);
            }
            else buildingToPlace = building;
        }
        else
        {
            placingBuilding = true;
            buildingToPlace = building;
        }
    }
}
