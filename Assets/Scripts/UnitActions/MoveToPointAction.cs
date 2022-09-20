using UnityEngine;

public class MoveToPointAction : UnitAction
{
    Vector3 point;

    public MoveToPointAction(Unit unit,  Vector3 point) : base(unit)
    {
        this.point = new Vector3(point.x, unit.transform.position.y, point.z);//Ignore y
    }

    public override void Update()
    {
        unit.NavigateToTarget(point);
        if (!unit.navigationRunning || unit.navMeshAgent.pathPending) return;
        if (unit.navMeshAgent.remainingDistance > unit.navMeshAgent.stoppingDistance) return;
        unit.StopNavigation();
        Deactivate();
    }
    
    protected override void StopIfCollidedWithTarget(Transform transform){}
}
