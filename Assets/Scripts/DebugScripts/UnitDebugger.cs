using UnityEngine;

public class UnitDebugger : MonoBehaviour
{
    public unitManager unitManager;
    
    Unit attachedUnit;

    public int teamId;
    public uint visibilityMask;
    public float[] detectionTimers;
    public string confirmedUnitName;

    private void Start()
    {
        attachedUnit = GetComponent<Unit>();
    }

    private void Update()
    {
        teamId = unitManager.getDebugTeam(attachedUnit);
        visibilityMask = unitManager.getDebugVisMask(attachedUnit);
        detectionTimers = unitManager.getDebugTime(attachedUnit);
        confirmedUnitName = unitManager.getDebugUnit(attachedUnit);
    }

    public string GetBinary()
    {
        return System.Convert.ToString(visibilityMask, 2).PadLeft(32, '0');
    }
}


