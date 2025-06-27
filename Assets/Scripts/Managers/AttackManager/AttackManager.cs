using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Linq;
using System.Xml;
using UnityEditor.SceneManagement;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


public class AttackManager : ModuleManager
{
    //This script will do all the processing of the unitsusing the attack data
    //object stored in each struct

    [SerializeField] private LayerMask unitLayer;

    [SerializeField] private int targetSkipSearchCooldown;
    Coroutine updateLoop;

    //first lets get all instance Ids we need that have this component
    public void Start()
    {
        managerType = "AttackManager";
        unitIds = GetIds();

        foreach(int unitId in unitIds)
        {
            AttackData attackData = manager.GetModuleData(unitId, managerType) as AttackData;

            //This works cus they are ref types even though they come from a struct
            attackData.attackComponent = manager.GetUnitData(unitId).unitScript.GetComponent<AttackComponent>();
            attackData.aimingPos = attackData.attackComponent.AimingPos;
        }
    }

    private void OnEnable()
    {
        updateLoop = StartCoroutine(UpdateLoop());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator UpdateLoop()
    {
        AttackData attackData;

        while (true)
        {
            foreach (int id in unitIds)
            {
                if (manager.TryGetModuleData(id, managerType, out ModuleData moduleData))
                {
                    attackData = moduleData as AttackData;

                    if (!levelManager.IsValidTarget(attackData.currentTargetId, managedTeam))
                    {
                        attackData.forcedTarget = false;
                        ResetTarget(attackData);
                    }

                    if (attackData.fireAtWill == false)
                    {
                        Debug.Log("Fire at will false");
                        continue;
                    }

                    else if (attackData.forcedTarget)
                    {
                        Debug.Log("Forced mode");
                        ForcedTargetMode(attackData);
                    }

                    else
                    {
                        AutoTargetMode(attackData);
                        Debug.Log("AutoMode");
                    }

                    if (attackData.inLOS && UpdateReloadTimer(attackData))
                    {
                        UnitFire(attackData);
                    }
                }
                else
                {
                    Debug.Log("HUHHH???");
                    RemoveId(id);
                }

                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForEndOfFrame();
        }
    }

    private void ForcedTargetMode(AttackData attackData)
    {
        //The can fire logic differs between modes. AutoTargetMode will find a new target if line of sight is not gained. 
    }

    private void AutoTargetMode(AttackData attackData)
    {
        if (attackData.currentTargetId == -1)
        {
            attackData.inLOS = false;

            if (attackData.targetSkipSearchCooldown > 0)
            {
                attackData.targetSkipSearchCooldown -= 1;
            }

            else if (FindNewTarget(attackData, out int targetId))
            {
                SetNewTarget(targetId, attackData);
            }

            else
            {
                attackData.targetSkipSearchCooldown = targetSkipSearchCooldown;
            }

            return;
        }

        else if (!InRangeCheck(attackData))
        {
            ResetTarget(attackData);
            return;
        }
        //Check if its in range still

        //LOS is only checked when the tank is aiming at the target to save on resources. 

        //updating if it can fire yet
        if (IsAimingAt(attackData))
        {
            //If its aiming at the target it will fire 
            if (CheckCurrentLOS(attackData))
            {
                attackData.inLOS = true;
            }

            else
            {
                ResetTarget(attackData);
                attackData.inLOS = false;
            }
        }

        else
        {
            attackData.inLOS = false;
        }
    }         
    

    private void ResetTarget(AttackData attackData)
    {
        attackData.forcedTarget = false;
        attackData.currentTargetId = -1;

        attackData.attackComponent.UpdateTurretRotation(null);
    }

    private void SetNewTarget(int targetId, AttackData attackData)
    {
        attackData.rayTarget = levelManager.GetUnitData(targetId).rayTarget;
        attackData.attackComponent.UpdateTurretRotation(attackData.rayTarget);
        attackData.currentTargetId = targetId;
    }

    public bool InRangeCheck(AttackData attackData)
    {
        return Vector3.Distance(attackData.aimingPos.position, attackData.rayTarget.position) < attackData.range;
    }

    public bool UpdateReloadTimer(AttackData attackData)
    {
        if (attackData.reloadTimer < Time.deltaTime)
        {
            attackData.reloadTimer = 0;
            return true;
        }
        else
        {
            attackData.reloadTimer -= Time.deltaTime;
            return false;
        }
    }

    private bool FindNewTarget(AttackData attackData, out int targetId)
    {
        Collider[] colliders = new Collider[200];

        int cols = Physics.OverlapSphereNonAlloc(attackData.aimingPos.position, attackData.range, colliders, unitLayer);
        Debug.Log("Colliders found: " +  cols);

        for (int i = 0; i < cols; i++)
        {
            if (colliders[i].transform.root.TryGetComponent<Unit>(out Unit unit))
            {
                if (!levelManager.IsValidTarget(unit.InstanceId, managedTeam))
                {
                    Debug.Log("Target not detected yet skipping");
                }

                else if (unit.TeamId != managedTeam)
                {
                    if (CheckLOS(attackData, unit))
                    {
                        Debug.Log("Enemy in LOS selected target");
                        targetId = unit.InstanceId;
                        return true;
                    }
                }
            }
        }

        targetId = -1;
        return false;
    }

    //This checks if its aiming at the target
    private bool IsAimingAt(AttackData attackData)
    {
        //check dotProduct first
        Vector3 vector = (attackData.rayTarget.transform.position - attackData.aimingPos.position).normalized;
        Vector3 currentVector = attackData.aimingPos.forward;
        if (Vector3.Dot(vector, currentVector) < 0.9999)
        {
            Debug.Log("Not facing target current DOT: " + Vector3.Dot(vector, currentVector));
            return false;
        }

        Debug.Log("facing target current DOT: " + Vector3.Dot(vector, currentVector));
        return true;
    }

    //This checks if it's within Line of sight. This is only checked when IsAimingAt results in true
    private bool CheckCurrentLOS(AttackData attackData)
    {
        Ray ray = new Ray(attackData.aimingPos.position, attackData.aimingPos.forward);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.transform.root.TryGetComponent<Unit>(out Unit unitCode))
            {
                if (unitCode.InstanceId == attackData.currentTargetId)
                {
                    return true;
                }
            }
        }

        return false;
    }

    //This is the same function but it checks the LOS as if it were aiming at it.
    private bool CheckLOS(AttackData attackData ,Unit targetScript)
    {
        Vector3 direction = targetScript.rayTarget.position - attackData.aimingPos.position;
        Debug.DrawRay(attackData.aimingPos.position, direction, Color.red, 10f);

        Ray ray = new Ray(attackData.aimingPos.position, direction);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
        {
            if (hit.collider.transform.root.TryGetComponent<Unit>(out Unit unitCode))
            {
                if (unitCode.InstanceId == targetScript.InstanceId)
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void UpdateTurretRot(AttackData attackData)
    {
        attackData.attackComponent.UpdateTurretRotation(attackData.rayTarget);
    }

    private void UnitFire(AttackData attackData)
    {
        AttackComponent attackComp = attackData.attackComponent;
        attackComp.FireGun();
        attackData.reloadTimer =+ attackData.reloadTime;
        //Later on it will wait till the shot has fired to damage the enemy. 

        levelManager.DamageUnit(attackData.currentTargetId, attackData.damage);
    }
}


//this will hold data for attacking. This will be stored in the unitmanager
//struct list which contains an array of all the components it has. 
[System.Serializable]
public class AttackData : ModuleData
{
    public AttackData()
    {
        moduleType = "AttackManager";
    }

    public override ModuleData Clone()
    {
        return new AttackData()
        {
            moduleType = moduleType,
            range = range,
            damage = damage,
            reloadTime = reloadTime,
        };
    }

    //the current targets instance Id
    public int currentTargetId = -1;
    public Transform rayTarget;

    //state booleans and counters
    public bool forcedTarget = false;
    public bool fireAtWill = true;
    public bool inLOS;
    public float reloadTimer = 0;
    public int targetSkipSearchCooldown;

    //weapon stats
    public float range = 0;
    public float damage = 0;
    public float reloadTime = 0;

    //Attack component. This contains important things like the AimingPos;
    public AttackComponent attackComponent;
    public Transform aimingPos;
}

