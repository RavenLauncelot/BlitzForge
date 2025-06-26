using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class ModuleManager : MonoBehaviour
{
    protected UnitManager manager;
    protected LevelManager levelManager;
    protected UnitManager.TeamId managedTeam;

    private bool initialised = false;

    [SerializeReference] protected int[] unitIds;

    protected string managerType;
    public string ModuleType
    {
        get { return managerType; }
    }

    public void InitModule(UnitManager unitManager, LevelManager LevelManager, UnitManager.TeamId teamid)
    {
        if (initialised)
        {
            return;
        }

        manager = unitManager;
        levelManager = LevelManager;
        initialised = true;
        managedTeam = teamid;

    }

    public int[] GetIds()
    {
        return manager.GetIdsWithModule(managerType);
    }

    public void RemoveId(int instanceId)
    {
        unitIds = unitIds.Where(val => val != instanceId).ToArray();
    }

    public virtual void SetCommand(CommandData command)
    {

    }

    public virtual void StopCommands(int[] ids)
    {

    }
}

[System.Serializable]
public class ModuleData
{
    public string moduleType;

    public virtual ModuleData Clone()
    {
        return new ModuleData();
    }
}
