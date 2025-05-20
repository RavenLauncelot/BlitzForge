using UnityEngine;

public class TurretAutoAim : MonoBehaviour
{
    [SerializeField] private Vector3 target;
    private Vector3 pivotXTarget;
    private Vector3 pivotYTarget;

    private UnitTargetting targettingComp;

    [SerializeField] private Transform pivotX;
    [SerializeField] private Transform pivotY;

    private void Start()
    {
        targettingComp = GetComponentInParent<UnitTargetting>();
    }

    private void Update()
    {
        target = targettingComp.GetTargetPos();

        pivotYTarget = new Vector3(target.x, pivotY.position.y, target.z);
        pivotY.LookAt(pivotYTarget);

        pivotXTarget = new Vector3(0, target.y - pivotX.position.y, Vector3.Distance(pivotX.position, target));
        pivotX.localRotation = Quaternion.LookRotation(pivotX.

        Debug.DrawRay(pivotY.position, pivotY.forward * 10, Color.yellow);
        Debug.DrawRay(pivotX.position, pivotX.forward * 10, Color.red);

        Debug.DrawRay(pivotY.position, pivotY.TransformPoint(pivotXTarget));
        Debug.Log("Pivot Y target: " + pivotYTarget + "    Pivot X target: " + pivotXTarget);

        
    }

    
}
