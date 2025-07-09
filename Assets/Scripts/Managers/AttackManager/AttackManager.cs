using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Linq;
using System.Xml;
using UnityEditor.SceneManagement;
using UnityEngine;


public class AttackManager : ModuleManager
{
    //This script will do all the processing of the unitsusing the attack data
    //object stored in each struct

    [SerializeField] private LayerMask unitLayer;

    [SerializeField] private int targetSkipSearchCooldown;
    Coroutine updateLoop;

    private Collider[] colliders;

    //first lets get all instance Ids we need that have this component
    public void Start()
    {
        colliders = new Collider[200];
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
        while (true)
        {
            foreach (UnitModule unitModule in managedModules)
            {
                AttackModule attackModule = unitModule as AttackModule;

                    if (!levelManager.IsValidTarget(attackModule.currentTargetId, managedTeam))
                    {
                        attackModule.forcedTarget = false;
                        ResetTarget(attackModule);
                    }

                    if (attackModule.fireAtWill == false)
                    {
                        Debug.Log("Fire at will false");
                        continue;
                    }

                    else if (attackModule.forcedTarget)
                    {
                        Debug.Log("Forced mode");
                        ForcedTargetMode(attackModule);
                    }

                    else
                    {
                        AutoTargetMode(attackModule);
                    }

                    if (attackModule.inLOS && UpdateReloadTimer(attackModule))
                    {
                        UnitFire(attackModule);
                    }

                yield return null;
            }

            yield return null;
        }
    }

    private void ForcedTargetMode(AttackModule attackData)
    {
        //The can fire logic differs between modes. AutoTargetMode will find a new target if line of sight is not gained. 
    }

    private void AutoTargetMode(AttackModule attackModule)
    {
        if (attackModule.currentTargetId == -1)
        {
            attackModule.inLOS = false;

            if (attackModule.targetSkipSearchCooldown > 0)
            {
                attackModule.targetSkipSearchCooldown -= 1;
            }

            else if (FindNewTarget(attackModule, out int targetId))
            {
                SetNewTarget(targetId, attackModule);
            }

            else
            {
                attackModule.targetSkipSearchCooldown = targetSkipSearchCooldown;
            }

            return;
        }

        else if (!InRangeCheck(attackModule))
        {
            ResetTarget(attackModule);
            return;
        }
        //Check if its in range still

        //LOS is only checked when the tank is aiming at the target to save on resources. 

        //updating if it can fire yet
        if (IsAimingAt(attackModule))
        {
            //If its aiming at the target it will fire 
            if (CheckCurrentLOS(attackModule))
            {
                attackModule.inLOS = true;
            }

            else
            {
                ResetTarget(attackModule);
                attackModule.inLOS = false;
            }
        }

        else
        {
            attackModule.inLOS = false;
        }
    }         
    

    private void ResetTarget(AttackModule attackModule)
    {
        attackModule.forcedTarget = false;
        attackModule.currentTargetId = -1;

        attackModule.UpdateTurretRotation(null);
    }

    private void SetNewTarget(int targetId, AttackModule attackModule)
    {
        attackModule.rayTarget = levelManager.GetUnitData(targetId).rayTarget;
        attackModule.UpdateTurretRotation(attackModule.rayTarget);
        attackModule.currentTargetId = targetId;
    }

    public bool InRangeCheck(AttackModule attackModule)
    {
        return Vector3.Distance(attackModule.AimingPos.position, attackModule.rayTarget.position) < attackModule.range;
    }

    public bool UpdateReloadTimer(AttackModule attackData)
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

    private int cols;
    private bool FindNewTarget(AttackModule attackModule, out int targetId)
    {
        //Collider[] colliders = new Collider[200];

        cols = Physics.OverlapSphereNonAlloc(attackModule.AimingPos.position, attackModule.range, colliders, unitLayer);

        for (int i = 0; i < cols; i++)
        {
            if (colliders[i].transform.TryGetComponent<Unit>(out Unit unit))
            {
                if (!levelManager.IsValidTarget(unit.InstanceId, managedTeam))
                {
                    continue;
                }

                else if (unit.TeamId != managedTeam)
                {
                    if (CheckLOS(attackModule, unit))
                    { 
                        targetId = unit.InstanceId;
                        return true;
                    }
                    else
                    {
                        continue;                    
                    }
                }
            }
        }

        targetId = -1;
        return false;
    }

    //This checks if its aiming at the target
    private bool IsAimingAt(AttackModule attackModule)
    {
        //check dotProduct first
        Vector3 vector = (attackModule.rayTarget.transform.position - attackModule.AimingPos.position).normalized;
        Vector3 currentVector = attackModule.AimingPos.forward;
        if (Vector3.Dot(vector, currentVector) < 0.9999)
        {
            Debug.Log("Not facing target current DOT: " + Vector3.Dot(vector, currentVector));
            return false;
        }

        Debug.Log("facing target current DOT: " + Vector3.Dot(vector, currentVector));
        return true;
    }

    //This checks if it's within Line of sight. This is only checked when IsAimingAt results in true
    private bool CheckCurrentLOS(AttackModule attackModule)
    {
        Ray ray = new Ray(attackModule.AimingPos.position, attackModule.AimingPos.forward);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.transform.root.TryGetComponent<Unit>(out Unit unitCode))
            {
                if (unitCode.InstanceId == attackModule.currentTargetId)
                {
                    return true;
                }
            }
        }

        return false;
    }

    //This is the same function but it checks the LOS as if it were aiming at it.
    private bool CheckLOS(AttackModule attackModule ,Unit targetScript)
    {
        Vector3 direction = targetScript.rayTarget.position - attackModule.AimingPos.position;
        Debug.DrawRay(attackModule.AimingPos.position, direction, Color.red, 10f);

        Ray ray = new Ray(attackModule.AimingPos.position, direction);
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

    private void UnitFire(AttackModule attackModule)
    {
        attackModule.FireGun();
        attackModule.reloadTimer =+ attackModule.reloadTime;
        //Later on it will wait till the shot has fired to damage the enemy. 

        levelManager.DamageUnit(attackModule.currentTargetId, attackModule.damage);
    }

    private void IsTargetValid(Unit unit)
    {

    }
}

