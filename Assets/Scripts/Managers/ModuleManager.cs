using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
using Sirenix.OdinInspector;

public class ModuleManager : MonoBehaviour
{
    private List<UnitModule> tempUnitModules;

    protected UnitManager manager;
    protected LevelManager levelManager;
    protected UnitManager.TeamId managedTeam;

    private bool initialised = false;

    [SerializeReference] protected int[] unitIds;
    [SerializeReference] protected UnitModule[] managedModules;
    protected Dictionary<int, UnitModule> moduleIdLookup;

    [SerializeField] protected string managerType;
    public string ManagerType
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

        tempUnitModules = new List<UnitModule>();
        moduleIdLookup = new Dictionary<int, UnitModule>();
    }

    public void StartModuleManager()
    {
        managedModules = tempUnitModules.ToArray();
    }

    public int[] GetIds()
    {
        return manager.GetIdsWithModule(managerType);
    }

    public void RegisterModule(UnitModule unitModule)
    {
        tempUnitModules.Add(unitModule);
        moduleIdLookup.Add(unitModule.InstanceId, unitModule);
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
