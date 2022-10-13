using UnityEngine;

public abstract class RTSFactionEntity : MonoBehaviour
{
    [Header("Setup")]
    public Faction faction;
    public DictionaryAmountPerResource resourcesNeededToCreateMe;
    
    protected Mesh mesh;

    [SerializeField] GameObject selectionCircle;

    protected virtual void Awake()
    {
        mesh = GetComponentInChildren<MeshFilter>().mesh;
    }

    protected virtual void Start()
    {
        transform.position += Vector3.up * 100;
    }

    public void ToggleSelectionCircle()
    {
        selectionCircle.SetActive(!selectionCircle.activeSelf);
    }
}
