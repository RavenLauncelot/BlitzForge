using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class UnitTargetting : UnitComponent, ILogicUpdate
{
    //tank info
    private Unit unit;

    //target info   
    public Unit currentTarget;

    //firing mode
    public bool forcedTarget;
    public bool fireAtWill;

    public bool canFire;

    //gun stats
    public float range;
    public float damage;
    public float reloadTime;

    //layer mask
    public LayerMask unitLayer;

    //reference points
    public Transform turretPivotPos;

    private void Start()
    {
        unit = GetComponent<Unit>();

        componentType = UnitComponents.UnitTargeting;
        if (turretPivotPos == null)
        {
            turretPivotPos = this.transform;
        }
    }

    public void SetForcedTarget(Unit target)
    {
        currentTarget = target;
        forcedTarget = true;
    }

    public void TimedLogicUpdate()
    {
        if (currentTarget != null & CanHitTarget(currentTarget) & fireAtWill)
        {
            canFire = true;           
        }

        else
        {
            canFire = false;

            if (currentTarget == null)
            {
                forcedTarget = false;
                currentTarget = FindTarget();
            }

            else if (!forcedTarget & !CanHitTarget(currentTarget))
            {
                currentTarget = FindTarget();
            }

            else
            {
                //do nothing fireatll was most likely set yo false
            }
        }
    }

    private Unit FindTarget()
    {
        Collider[] inRange = Physics.OverlapSphere(transform.position, range, unitLayer);

        foreach (Collider collider in inRange)
        {           
            Ray ray = new Ray(turretPivotPos.position, collider.transform.position - turretPivotPos.position);
            Debug.DrawRay(turretPivotPos.position, collider.transform.position - turretPivotPos.position, Color.yellow, 4f);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform.TryGetComponent<Unit>(out Unit selectedUnit) == collider.gameObject.GetComponent<Unit>())
                {
                    if (selectedUnit != this)
                    {
                        //raycast has hit enemy within range. enemy selection successful
                        Debug.Log("Found unit distance: " + Vector3.Distance(collider.transform.position, turretPivotPos.position));
                        return selectedUnit;
                    }                                  
                }
            }
        }

        Debug.Log("No valid targets: " + this.gameObject.name);
        return null;
    }

    private bool CanHitTarget(Unit target)
    {    
        if (target == null)
        {
            return false;
        }

        Ray ray = new Ray(turretPivotPos.position, target.transform.position - turretPivotPos.position);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.distance > range)
            {
                Debug.Log("Target out of range");
                return false;
            }
            else if (hit.collider.TryGetComponent<Unit>(out Unit unitTarg) == false)
            {
                Debug.Log("Target blocked");
                return false;
            }
            else if (hit.collider.TryGetComponent(out Unit unitTarg2) == true && unitTarg2.TeamId == unit.TeamId)
            {
                Debug.Log("Friendly is within line of fire");
                return false;
            }
            else
            {
                return true;
            }            
        }

        return false;
    }

    public void OnDrawGizmos()
    {
        if (currentTarget != null)
        {
            Ray r = new Ray(transform.position, currentTarget.transform.position - transform.position);
            Gizmos.DrawRay(r);
        }

        Gizmos.DrawWireSphere(transform.position, range);
        
    }

    public Transform GetTargetTrans()
    {
        return currentTarget.transform;
    }

    //private IEnumerator FireTimer()
    //{
    //    while (true)
    //    {
    //        if (canFire)
    //        {
    //            //fire bullet innit
    //            Fire();
    //            yield return new WaitForSeconds(reloadTime);
    //        }

    //        yield return new WaitForEndOfFrame();
    //    }
    //}

    //private void Fire()
    //{
    //    Vector3 Gunvector = currentTarget.transform.position - transform.position;
        
    //    Ray tankShot = new Ray(turretPivotPos.transform.position, Gunvector);

    //    if(Physics.Raycast(tankShot, out RaycastHit hit))
    //    {
    //        if (hit.collider.TryGetComponent<IDamageable>(out IDamageable dealDamage))
    //        {
    //            dealDamage.Damage(damage);
    //        }
    //    }
    //}
}
