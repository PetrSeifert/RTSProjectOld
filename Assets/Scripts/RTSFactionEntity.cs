using UnityEngine;

public abstract class RTSFactionEntity : MonoBehaviour
{
    [Header("Setup")]
    public Faction faction;
    public DictionaryAmountPerResource resourcesNeededToCreateMe;
    public Renderer renderer;
}
