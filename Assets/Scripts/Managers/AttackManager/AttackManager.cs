using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;


public class AttackManager : ModuleManager
{
    //This script will do all the processing of the unitsusing the attack data
    //object stored in each struct

    [SerializeField] private LayerMask unitLayer;
    [SerializeField] private LayerMask everything;

    [SerializeField] private int targetSkipSearchCooldown;
    Coroutine updateLoop;

    private Collider[] colliders;

    AttackModule[] attackModules;

    [SerializeField] VisibilityManager visibilityManager;
    [SerializeField] MovementManager movementManager;

    public override void InitModuleManager()
    {
        base.InitModuleManager();

        attackModules = managedModules.Cast<AttackModule>().ToArray();
        colliders = new Collider[200];
    }

    public override void StartModuleManager()
    {
        base.StartModuleManager();

        updateLoop = StartCoroutine(UpdateLoop());
    }

    private void OnEnable()
    {
        if (managerStarted)
        {
            updateLoop = StartCoroutine(UpdateLoop());
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public override void UnregisterModule(int id)
    {
        StopAllCoroutines();

        attackModules = attackModules.Where(val => val.InstanceId != id).ToArray();
        managedModules = managedModules.Where(val => val.InstanceId != id).ToArray();

        updateLoop = StartCoroutine(UpdateLoop());
    }

    public override void StopCommands(int[] ids)
    {
        AttackModule attackModule;

        foreach(int id in ids)
        {
            if (moduleIdLookup.TryGetValue(id, out UnitModule module))
            {
                attackModule = module as AttackModule;
                attackModule.forcedTarget = false;
            }
        }
    }

    public override void SetCommand(CommandData command)
    {
        base.SetCommand(command);

        if (command.commandType == "AttackCommand")
        {
            if (command.targettedUnits.Length == 0)
            {
                return;
            }

            AttackCommand(command.selectedUnits, command.targettedUnits[0]);
        }
    }

    public void AttackCommand(int[] instanceIds, int target)
    {
        Unit targetUnit;

        if (!manager.unitIdLookUp.TryGetValue(target, out targetUnit))
        {
            Debug.Log("Invalid target");
            return;
        }

        for (int i = 0;  i < instanceIds.Length; i++)
        {
            moduleIdLookup.TryGetValue(instanceIds[i], out UnitModule module);
            AttackModule attackModule = module as AttackModule;

            ResetTarget(attackModule);

            attackModule.forcedTarget = true;
            SetNewTarget(targetUnit, attackModule);
        }
    }

    private void Update()
    {
        if (managerStarted)
        {
            foreach (AttackModule attackModule in attackModules)
            {
                UpdateReloadTimer(attackModule);
            }
        }
    }

    private IEnumerator UpdateLoop()
    {
        while (true)
        {          
            foreach (AttackModule attackModule in attackModules)
            {
                if (!attackModule.fireAtWill)
                {
                    if (attackModule.currentTarget != null)
                    {
                        ResetTarget(attackModule);
                    }

                    continue;
                }

                if (!IsTargetValid(attackModule.currentTarget, attackModule))
                {
                    ResetTarget(attackModule);
                    attackModule.forcedTarget = false;
                    AutoTargetMode(attackModule);
                }

                else if (attackModule.forcedTarget)
                {
                    ForcedTargetMode(attackModule);
                }

                else
                {
                    AutoTargetMode(attackModule);
                }             

                if (attackModule.inLOS && attackModule.reloadTimer == 0)
                {
                    UnitFire(attackModule);
                }


                yield return null;
            }

            yield return null;
        }
    }

    //This will mainly handle movement 
    private void ForcedTargetMode(AttackModule attackData)
    {
        //if it is aiming at the target it will then check LOS. if not it will return and wait
        if (IsAimingAt(attackData))
        {
            if (CheckCurrentLOS(attackData) && InRangeCheck(attackData))
            {
                //if not within line of sight it will find a new position. If not it won't 
                attackData.inLOS = true;
            }
            else
            {
                attackData.inLOS = false;
            }
        }
        else
        {
            return;
        }

        //If is still moving we don't check anything till it's finished moving 
        if (!attackData.moveMod.ReachedTarget)
        {
            return;
        }

        //If not in range move to closest point in range
        if (!InRangeCheck(attackData))
        {
            Debug.Log("Not in range");

            Vector3 positonToMove = attackData.targetRayCheck.position;

            Vector3 vectorFromTarget = attackData.AimingPos.position - positonToMove;
            vectorFromTarget.Normalize();

            positonToMove = positonToMove + (vectorFromTarget * (attackData.range - 1)); //the minus 1 is to make sure it's in range

            if (NavMesh.SamplePosition(positonToMove, out NavMeshHit hit, 10f, everything))
            {
                CommandData command = new CommandData()
                {
                    commandType = "MovementCommand",
                    targettedArea = new Vector3[] { hit.position, hit.position },
                    selectedUnits = new int[] { attackData.InstanceId }
                };

                movementManager.SetCommand(command);

                return;
            }
            else
            {
                return;
            }
        }

        //if it's not within LOS but within range it will move closer a certain amount unless it is within the min range
        if (!attackData.inLOS)
        {
            Debug.Log("Not in LOS");

            float currentDistance = Vector3.Distance(attackData.AimingPos.position, attackData.targetRayCheck.position);

            CommandData command = new CommandData()
            {
                commandType = "MovementCommand",
                selectedUnits = new int[] { attackData.InstanceId }
            };

            Vector3 positionToMove = Vector3.zero;

            //If within 5 units it will just find a random positon within 5 otherwise it will find a postion that's 10% closer
            if (currentDistance > attackData.range)
            {
                positionToMove = NavmeshTools.RandomPointInsideNav(attackData.AimingPos.position, attackData.range-1, 10);

                command.targettedArea = new Vector3[] { positionToMove, positionToMove };

                movementManager.SetCommand(command);
            }
            //if it isnt it will find a postion 10% closer than previous
            else
            {
                positionToMove = NavmeshTools.RandomPointInsideNav(attackData.AimingPos.position, currentDistance * 0.7f, 10);

                command.targettedArea = new Vector3[] { positionToMove, positionToMove };

                movementManager.SetCommand(command);
            }
        }


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

        if (visibilityManager.IsTargetDetected(targetUnit.InstanceId, attackModule.TeamId, 0.1f))
        {
            return true;
        }

        return false;      
    }

    private void ResetTarget(AttackModule attackModule)
    {
        if (attackModule == null)
        {
            return;
        }

        attackModule.forcedTarget = false;
        attackModule.currentTarget = null;
        attackModule.inLOS = false;
        attackModule.targetRayCheck = null;

        attackModule.UpdateTurretRotation(null);
    }

    private void SetNewTarget(Unit target, AttackModule attackModule)
    {
        attackModule.targetRayCheck = target.rayTarget;
        attackModule.UpdateTurretRotation(attackModule.targetRayCheck);
        attackModule.currentTarget = target;
    }

    public bool InRangeCheck(AttackModule attackModule)
    {
        return Vector3.Distance(attackModule.AimingPos.position, attackModule.targetRayCheck.position) < attackModule.range;
    }

    public void UpdateReloadTimer(AttackModule attackData)
    {
        if (attackData.reloadTimer < Time.deltaTime)
        {
            attackData.reloadTimer = 0;
            return;
        }

        attackData.reloadTimer -= Time.deltaTime;
        
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
        Vector3 vector = (attackModule.targetRayCheck.transform.position - attackModule.AimingPos.position).normalized;
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

