using System.Collections;
using System.Linq;
using System.Xml;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;


public class AttackManager : ModuleManager
{
    //This script will do all the processing of the unitsusing the attack data
    //object stored in each struct

    [SerializeField] private LayerMask unitLayer;

    [SerializeField] private int targetSkipSearchCooldown;
    Coroutine updateLoop;

    //These are temporary values
    private AttackData attackData;
    private int currentUnitId;
    private int currentTargetId;
    private UnitManager.UnitData currentUnitData;
    private UnitManager.UnitData targetUnitData;
    

    //first lets get all instance Ids we need that have this component
    public void Start()
    {
        managerType = "AttackManager";
        unitIds = GetIds();

        foreach(int unitId in unitIds)
        {
            AttackData attackData = manager.GetModuleData(unitId, managerType) as AttackData;

            //This works cus they are ref types even though they come from a struct
            attackData.attackComponent = manager.GetUnitDataReadOnly(unitId).unitScript.GetComponent<AttackComponent>();
            attackData.unitScript = attackData.attackComponent.GetComponent<Unit>();
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

    //this will get changed to slower update later
    private IEnumerator UpdateLoop()
    {
        while (true)
        {
            attackData = null;

            foreach (int id in unitIds)
            {
                currentUnitId = id;

                if (manager.TryGetModuleData(id, managerType, out ModuleData moduleData))
                {
                    //UnitData variables cannot be updated. as they are value types
                    //Module data can thuogh cus they are reference types. We love ref types but they slow, stinky. 

                    attackData = moduleData as AttackData;
                    currentTargetId = attackData.currentTargetId;
                    currentUnitData = manager.GetUnitDataReadOnly(currentUnitId);
                    targetUnitData = levelManager.GetUnitData(currentTargetId);


                    UpdateReloadTimer();


                    if (attackData.forcedTarget)
                    {
                        ForcedTargetMode();
                    }

                    else
                    {
                        AutoTargetMode();
                    }

                    UpdateCanFire();
                    UpdateAttackComp();

                    if (attackData.canFire)
                    {
                        UnitFire(currentUnitId, attackData);
                    }
                }

                else
                {
                    RemoveId(id);
                    continue;
                }

                //Might add the ability to adjust this delay with doing it in batches e.g.
                //Doing 10 units then waiting a frame and being able to adjust this. 
                yield return new WaitForEndOfFrame();
            }

            yield return new WaitForEndOfFrame();
        }
    }

    public void ForcedTargetMode()
    {
        if (!levelManager.IsValidTarget(attackData.currentTargetId, managedTeam))
        {
            attackData.forcedTarget = false;
            attackData.currentTargetId = 0;

            return;
        }

        //Do normal targetting
        targetUnitData = levelManager.GetUnitData(attackData.currentTargetId);
        currentUnitData = manager.GetUnitDataReadOnly(currentUnitId);

        //Movement logic here later 
        //Will move within range of enemy if line of sight is still impacted move closer. 
    }

    public void AutoTargetMode()
    {
        if (attackData.targetSkipSearchCooldown > 0)
        {
            if (attackData.targetSkipSearchCooldown - 1 < 0)
            {   
                attackData.targetSkipSearchCooldown = 0;
            }

            else
            {
                attackData.targetSkipSearchCooldown -= 1;
            }

            return;
        }

        if (!CanAimAt(currentUnitId, targetUnitData.unitScript))
        {
            if (FindTarget(currentUnitData, attackData, out int newTarget))
            {
                attackData.currentTargetId = newTarget;
                attackData.targetUnitScript = levelManager.GetUnitData(newTarget).unitScript;
            }

            else
            {
                Debug.Log("Failed to find targets");
                //failed to find any targets

                attackData.targetSkipSearchCooldown = targetSkipSearchCooldown;  //if it fails to find a target it will wait till it can again.
                attackData.currentTargetId = 0;
                attackData.targetUnitScript = null;
            }
        }

        else
        {
            //do nothing you're good mate
        }
    }

    public void UpdateReloadTimer()
    {
        if (attackData.reloadTimer < Time.deltaTime)
        {
            attackData.reloadTimer = 0;
        }
        else
        {
            attackData.reloadTimer -= Time.deltaTime;
        }
    }

    public void UpdateCanFire()
    {
        attackData.canFire = currentTargetId != 0 &&
            IsAimingAt() &&
            attackData.reloadTimer <= 0;
    }

    public void UpdateAttackComp()
    {
        if (attackData.currentTargetId == 0)
        {
            AttackComponent attackComp = attackData.attackComponent;
            attackComp.UpdateTurretRotation(Vector3.down);
        }
        else
        {
            AttackComponent attackComp = attackData.attackComponent;
            attackComp.UpdateTurretRotation(attackData.targetUnitScript.rayTarget.position);
        }
    }

    private bool FindTarget(UnitManager.UnitData unitData, AttackData attackData, out int targetId)
    {
        //Important notice 
        //REMEMBER that unitdata is a copy.
        
        Vector3 aimingPos = unitData.aimingPos.position;

        Collider[] unitsInRange = new Collider[100];
        int unitCount = Physics.OverlapSphereNonAlloc(unitData.unitScript.transform.position, attackData.range, unitsInRange, unitLayer);

        for (int i = 0; i < unitCount; i++)
        {
            if (unitsInRange[i].transform.root.TryGetComponent<Unit>(out Unit targetCode))
            {
                if (targetCode.TeamId != managedTeam && levelManager.IsValidTarget(targetCode.InstanceId, managedTeam))
                {
                    if (CanAimAt(currentUnitId, targetCode))
                    {
                        targetId = targetCode.InstanceId;
                        return true;
                    }

                }
            }

            else
            {
                continue;
            }
        }

        targetId = 0;
        return false;
    }

    //This checks if it will be able to fire at the enemy tank once it has aimed towards the enemy. 
    private bool CanAimAt(int currentUnitId, Unit potentialTarget)
    {
        if (potentialTarget == null)
        {
            return false;
        }
        
        Ray ray = new Ray(currentUnitData.aimingPos.position, potentialTarget.rayTarget.position - currentUnitData.aimingPos.position);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.transform.root.TryGetComponent<Unit>(out Unit unitOut))
            {
                if (unitOut == potentialTarget && hit.distance < attackData.range)
                {
                    return true;
                }
            }

            Debug.DrawRay(currentUnitData.aimingPos.position, potentialTarget.rayTarget.position - currentUnitData.aimingPos.position);
        }


        return false;
    }

    //This checks if it can fire at the enemy tank and hit it
    private bool IsAimingAt()
    {
        Ray ray = new Ray(currentUnitData.aimingPos.position, currentUnitData.aimingPos.forward);
        if (Physics.Raycast(ray,out RaycastHit hit))
        {
            if (hit.collider.transform.root.TryGetComponent<Unit>(out Unit unitCode))
            {
                if (unitCode.InstanceId == currentTargetId)
                {
                    return true;
                }
            }           
        }

        return false;
    }

    private void UnitFire(int unitToUpdate, AttackData attackData)
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
    public int currentTargetId = 0;
    public Unit targetUnitScript;

    //state booleans and counters
    public bool forcedTarget = false;
    public bool fireAtWill = true;
    public bool canFire;
    public float reloadTimer = 0;
    public int targetSkipSearchCooldown;

    //weapon stats
    public float range = 0;
    public float damage = 0;
    public float reloadTime = 0;

    //things connected to the unit
    public AttackComponent attackComponent;
    public Unit unitScript;
}

