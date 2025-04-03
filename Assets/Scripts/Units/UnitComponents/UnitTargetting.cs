using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class UnitTargetting : UnitComponent, ILogicUpdate
{
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
    public LayerMask layerMask;

    //reference points
    public Transform turretPivotPos;

    private void Start()
    {
        componentType = UnitComponents.UnitTargeting;
    }

    public void SetForcedTarget(Unit target)
    {
        currentTarget = target;
        forcedTarget = true;
    }

    public void TimedLogicUpdate()
    {
        if(currentTarget == null)
        {
            forcedTarget = false;
            canFire = false;
            Debug.Log("Current Target null fnding new target");
            currentTarget = FindTarget();
        }

        else if(LineOfSight())
        {
            if (fireAtWill)
            {
                canFire = true;
            }
            else if (forcedTarget)
            {
                canFire = true;
            }
            else
            {
                canFire = false;
            }
        }

        else if(forcedTarget)
        {
            canFire = false;
            return;
        }

        else
        {
            Debug.Log("Target out of range and out of sight getting new target");
            canFire = false;
            currentTarget = FindTarget();
        }

        Debug.Log("Logic update");
    }

    private Unit FindTarget()
    {
        Collider[] inRange = Physics.OverlapSphere(transform.position, range, layerMask);

        foreach (Collider collider in inRange)
        {           
            Ray ray = new Ray(turretPivotPos.position, collider.transform.position - turretPivotPos.position);

            if (Physics.Raycast(ray, out RaycastHit hit, layerMask))
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

    private bool LineOfSight()
    {
        if (currentTarget == null)
        {
            return false;
        }

        Ray ray = new Ray(turretPivotPos.position, currentTarget.transform.position - turretPivotPos.position);

        if (Physics.Raycast(ray, out RaycastHit hit, layerMask))
        {
            if (hit.transform.TryGetComponent<Unit>(out Unit selectedUnit) == currentTarget.gameObject.GetComponent<Unit>() && hit.distance < range)
            {
                Debug.Log("Target in line of sight and in range Range: " + hit.distance);
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
