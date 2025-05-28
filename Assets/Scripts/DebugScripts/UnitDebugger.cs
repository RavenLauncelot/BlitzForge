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

    private void Start()
    {
        attachedUnit = GetComponent<Unit>();
        unitManager = attachedUnit.unitManager;
    }

    private void Update()
    {
        teamId = unitManager.getDebugTeam(attachedUnit);
        visibilityMask = unitManager.getDebugVisMask(attachedUnit);
        detectionTimers = unitManager.getDebugTime(attachedUnit);
        confirmedUnitName = unitManager.getDebugUnit(attachedUnit);
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attachedUnit.detectionPos.position, attachedUnit.detectionRange);
    }
}


