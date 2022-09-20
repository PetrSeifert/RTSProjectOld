using UnityEngine;

public abstract class UnitAction
{
    public bool prioritySet;
    
    protected Unit unit;
    protected Transform targetTransform;

    protected UnitAction(Unit unit)
    {
        this.unit = unit;
        unit.navigationRunning = false;
        unit.onCollisionEntry.AddListener(StopIfCollidedWithTarget);
    }

    public void Deactivate()
    {
        unit.onCollisionEntry.RemoveListener(StopIfCollidedWithTarget);
        unit.action = null;
    }
    
    /// <summary>
    /// Isn't MonoBehaviour Update().
    /// Should be called by unit.
    /// </summary>
    public abstract void Update();

    protected abstract void StopIfCollidedWithTarget(Transform transform);
}
