using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class AttackManager : ManagerModule
{
    //This script will do all the processing of the unitsusing the attack data
    //object stored in each struct

    public LayerMask unitLayer;

    public int[] unitIds;

    public int updatePerSec = 1;

    //first lets get all instance Ids we need that have this component
    public void Start()
    {
        unitIds = GetIds(UnitComponent.ComponentType.AttackComp);

        //StartCoroutine(TimedUpdate());
    }

    //private IEnumerator TimedUpdate()
    //{
    //    int currentIndex;
    //    AttackData attackData = null;

    //    while (true)
    //    {
    //        foreach (int id in unitIds)
    //        {
    //            currentIndex = manager.getUnitDataIndex(id);

    //            //getting the attack data object
    //            if (manager.getCompData(id, UnitComponent.ComponentType.AttackComp) is AttackData AttackData)
    //            {
    //                attackData = AttackData;
    //            }
    //            else
    //            {
    //                attackData = null;
    //                Debug.Log("AttackManager skipping couldn't get unit data");
    //                continue;
    //            }



    //            if (attackData.currentTargetId != 0 & CanHitTarget(manager.unitData[currentIndex], attackData, levelManager.getUnitData(attackData.currentTargetId)) & attackData.fireAtWill)
    //            {
    //                attackData.canFire = true;
    //            }

    //            else
    //            {
    //                attackData.canFire = false;

    //                if (attackData.currentTargetId == 0)
    //                {
    //                    attackData.forcedTarget = false;
    //                    attackData.currentTargetId = FindTarget(manager.unitData[currentIndex], attackData);
    //                }

    //                else if (!attackData.forcedTarget & !CanHitTarget(manager.unitData[currentIndex], attackData, levelManager.getUnitData(attackData.currentTargetId)))
    //                {
    //                    attackData.currentTargetId = FindTarget(manager.unitData[currentIndex], attackData);
    //                }

    //                else
    //                {
    //                    //do nothing fireatll was most likely set yo false
    //                }
    //            }
    //        }

    //        yield return new WaitForSeconds(1 / updatePerSec);
    //    }
    //}

    public void Update()
    {
        int currentUnitDataIndex;
        AttackData attackData = null;

        foreach (int id in unitIds)
        {
            currentUnitDataIndex = manager.unitIndexLookup[id];

            //getting the attack data object
            if (manager.getCompData(id, UnitComponent.ComponentType.AttackComp) is AttackData AttackData)
            {
                attackData = AttackData;
            }
            else
            {
                attackData = null;
                Debug.Log("AttackManager skipping couldn't get unit data");
                continue;
            }



            if (attackData.currentTargetId != 0 & CanHitTarget(manager.unitData[currentUnitDataIndex], attackData, levelManager.getUnitData(attackData.currentTargetId)) & attackData.fireAtWill)
            {
                attackData.canFire = true;
            }

            else
            {
                attackData.canFire = false;

                if (attackData.currentTargetId == 0)
                {
                    attackData.forcedTarget = false;
                    attackData.currentTargetId = FindTarget(manager.unitData[currentUnitDataIndex], attackData);
                }

                else if (!attackData.forcedTarget & !CanHitTarget(manager.unitData[currentUnitDataIndex], attackData, levelManager.getUnitData(attackData.currentTargetId)))
                {
                    attackData.currentTargetId = FindTarget(manager.unitData[currentUnitDataIndex], attackData);
                }

                else
                {
                    //do nothing fireatll was most likely set yo false
                }
            }
        }
    }

    private int FindTarget(UnitManager.UnitData unitData, AttackData attackData)
    {
        //Important notice 
        //REMEMBER that unitdata is a copy.
        
        Vector3 aimingPos = unitData.aimingPos.position;
        Collider[] inRange = Physics.OverlapSphere(unitData.unitScript.transform.position, attackData.range, unitLayer);

        foreach (Collider collider in inRange)
        {
            Unit unitCode;

            if (collider.TryGetComponent<Unit>(out Unit UnitCode))
            {
                if (UnitCode.TeamId == unitData.teamId)
                {
                    //Debug.Log("Stop hitting yourself");
                    continue;
                }

                unitCode = UnitCode;
                
            }
            else
            {
                continue;
            }

            Ray ray = new Ray(aimingPos, unitCode.rayTarget.position - aimingPos);
            Debug.DrawRay(aimingPos, unitCode.rayTarget.position - aimingPos, Color.yellow, 4f);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform.TryGetComponent<Unit>(out Unit selectedUnit) && selectedUnit == unitCode)
                {
                    //raycast has hit enemy within range. enemy selection successful
                    Debug.Log("Team: " + unitData.teamId + "Found target " + gameObject.name);
                    return selectedUnit.instanceId;
                }
            }
        }

        Debug.Log("No valid targets" + gameObject.name);
        return 0;
    }

    private bool CanHitTarget(UnitManager.UnitData currentUnit, AttackData attackData, UnitManager.UnitData target)
    {
        //Important notice 
        //REMEMBER that unitdata is a copy. 

        if (attackData.currentTargetId == 0)
        {
            return false;
        }

        Vector3 aimingRayPos = currentUnit.aimingPos.position;
        Vector3 targetPos = target.unitScript.transform.position;


        Ray ray = new Ray(aimingRayPos, targetPos - aimingRayPos);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.distance > attackData.range)
            {
                Debug.Log("Target out of range" + gameObject.name);
                return false;
            }
            else if (hit.collider.TryGetComponent<Unit>(out Unit unitTarg) == false)
            {
                Debug.Log("Target blocked" + gameObject.name);
                return false;
            }
            else if (hit.collider.TryGetComponent(out Unit unitTarg2) == true && unitTarg2.TeamId == currentUnit.teamId)
            {
                Debug.Log("Friendly is within line of fire" + gameObject.name);
                return false;
            }
            else
            {
                return true;
            }
        }

        return false;
    }
}

//this will hold data for attacking. This will be stored in the unitmanager
//struct list which contains an array of all the components it has. 
public class AttackData : UnitComponent
{
    public AttackData()
    {
        compType = ComponentType.AttackComp;
    }

    //the current targets instance Id
    public int currentTargetId;

    public bool forcedTarget;
    public bool fireAtWill;
    public bool canFire;

    public float range = 10;
    public float damage;
    public float reloadTime;
}