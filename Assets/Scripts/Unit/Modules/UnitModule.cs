using UnityEngine;

public class UnitModule : MonoBehaviour
{
    //Id's will still be used for identification. This is so we can use dictionaries to find the correct modules data. 
    //Instead of checking each individual module for the relevant data.

    protected int instanceId;
    public int InstanceId
    {
        get { return instanceId; }
    }

    [SerializeField] protected string[] targetModuleManager;
    public string[] TargetModuleManager
    {
        get { return targetModuleManager; }
    }

    public void InitModule(int instanceID, UnitManager.TeamId team)
    {
        instanceId = instanceID;
        teamId = team;
    }

    public virtual void CustomInit()
    {

    }

    protected UnitManager.TeamId teamId;
    public UnitManager.TeamId TeamId
    {
        get { return teamId; }
    }
}
