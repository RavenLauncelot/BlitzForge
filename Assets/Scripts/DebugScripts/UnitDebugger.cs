using System.Net.Mail;
using UnityEngine;
using static UnitManager;

public class UnitDebugger : MonoBehaviour
{
    public UnitManager unitManager;
    
    Unit attachedUnit;

    public int teamId;
    public bool[] visibilityMask;
    public float[] detectionTimers;
    public string confirmedUnitName;

    public Vector3 aimingPos;
    public Vector3 observingPos;

    //attack component
    public float range;
    public float damage;
    public float reloadTime;
    public int targetId;
    public bool canfire;
    public bool fireAtWill;
    public bool forcedTarget;

    public void Start()
    {
        attachedUnit = GetComponent<Unit>();
        unitManager = attachedUnit.unitManager;
    }

    private void Update()
    {
        UnitData unitDataCopy = unitManager.unitData[unitManager.unitDataIndexLookup[attachedUnit.instanceId]];

        teamId = (int)unitDataCopy.teamId;
        visibilityMask = unitDataCopy.teamVisibility;
        detectionTimers = unitDataCopy.detectedTimers;
        confirmedUnitName = unitDataCopy.unitScript.gameObject.name;
        aimingPos = unitDataCopy.aimingPos.position;
        observingPos = unitDataCopy.observingPos.position;

        //attack module debug
        AttackData attackData = unitManager.getCompData(attachedUnit.instanceId, ManagerData.ModuleType.AttackModule) as AttackData;

        range = attackData.range;
        damage = attackData.damage;
        reloadTime = attackData.reloadTime;
        targetId = attackData.currentTargetId;
        canfire = attackData.canFire;
        fireAtWill = attackData.fireAtWill;
        forcedTarget = attackData.forcedTarget;
        
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attachedUnit.aimingPos.position, attachedUnit.detectionRange);
    }
}


