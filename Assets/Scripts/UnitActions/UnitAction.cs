using UnityEngine;

public abstract class UnitAction
{
    public bool prioritySet;
    
    protected Unit unit;
    protected Transform targetTransform;

    protected UnitAction(Unit unit)
    {
        this.unit = unit;
    }

    public void Deactivate()
    {
        unit.action = null;
    }
    
    /// <summary>
    /// Isn't MonoBehaviour Update().
    /// Should be called by unit.
    /// </summary>
    public abstract void Update();
}
