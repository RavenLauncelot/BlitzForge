using Mono.Cecil.Cil;
using System;
using System.Collections;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

                //if (attackModule.currentTarget == null && attackModule.fireAtWill == true)
                //{
                //    //Skip past other checks 
                //    AutoTargetMode(attackModule);
                //}

                if (!attackModule.fireAtWill)
                {
                    if (attackModule.currentTarget != null)
                    {
                        ResetTarget(attackModule);
                    }

                    UpdateReloadTimer(attackModule);
                    continue;
                }

                if (attackModule.forcedTarget)
                {
                    if (IsTargetValid(attackModule.currentTarget, attackModule))
                    {
                        ForcedTargetMode(attackModule);
                    }

                    else
                    {
                        ResetTarget(attackModule);
                        attackModule.forcedTarget = false;
                        AutoTargetMode(attackModule);
                    }
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
        if (attackModule.currentTarget == null)
        {
            attackModule.inLOS = false;

            if (attackModule.targetSkipSearchCooldown > 0)
            {
                attackModule.targetSkipSearchCooldown -= 1;
            }

            else if (FindNewTarget(attackModule, out Unit target))
            {
                SetNewTarget(target, attackModule);
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
    
    private bool IsTargetValid(Unit targetUnit, AttackModule attackModule)
    {
        if (targetUnit == null)
        {
            return false;
        }

        if (!targetUnit.IsAlive)
        {
            return false;
        }

        if (manager.IsTargetDetected(targetUnit.InstanceId, attackModule.TeamId))
        {
            return true;
        }

        return false;      
    }

    private void ResetTarget(AttackModule attackModule)
    {
        attackModule.forcedTarget = false;
        attackModule.currentTarget = null;

        attackModule.UpdateTurretRotation(null);
    }

    private void SetNewTarget(Unit target, AttackModule attackModule)
    {
        attackModule.TargetRayCheck = target.aimingPos;
        attackModule.UpdateTurretRotation(attackModule.TargetRayCheck);
        attackModule.currentTarget = target;
    }

    public bool InRangeCheck(AttackModule attackModule)
    {
        return Vector3.Distance(attackModule.AimingPos.position, attackModule.TargetRayCheck.position) < attackModule.range;
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
    private bool FindNewTarget(AttackModule attackModule, out Unit target)
    {
        Collider[] colliders = new Collider[200];

        cols = Physics.OverlapSphereNonAlloc(attackModule.AimingPos.position, attackModule.range, colliders, unitLayer);

        for (int i = 0; i < cols; i++)
        {
            if (colliders[i].transform.root.TryGetComponent<Unit>(out Unit unit))
            {
                if (!IsTargetValid(unit, attackModule))
                {
                    continue;
                }

                else if (unit.TeamId != attackModule.TeamId)
                {
                    if (!CheckLOS(attackModule, unit))
                    {
                        continue;                    
                    }

                    target = unit;
                    return true;                          
                }
            }
        }

        target = null;
        return false;
    }

    //This checks if its aiming at the target
    private bool IsAimingAt(AttackModule attackModule)
    {
        //check dotProduct first
        Vector3 vector = (attackModule.TargetRayCheck.transform.position - attackModule.AimingPos.position).normalized;
        Vector3 currentVector = attackModule.AimingPos.forward;
        if (Vector3.Dot(vector, currentVector) < 0.9999)
        {
            return false;
        }

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
                if (unitCode.InstanceId == attackModule.currentTarget.InstanceId)
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
        Debug.DrawRay(attackModule.AimingPos.position, direction.normalized * attackModule.range, Color.yellow, 1f);
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

        manager.DamageUnit(attackModule.currentTarget.InstanceId, attackModule.damage);
    }
}

