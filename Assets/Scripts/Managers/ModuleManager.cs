using System;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor.Modules;

public class ModuleManager : MonoBehaviour
{
    private List<UnitModule> tempUnitModules;

    protected UnitManager manager;
    protected LevelManager levelManager;

    [SerializeReference] protected UnitModule[] managedModules;
    protected Dictionary<int, UnitModule> moduleIdLookup;

    [SerializeField] protected ModuleType managerType;
    public ModuleType ManagerType
    {
        get { return managerType; }
    }

    public void InitModuleManager(UnitManager unitManager, LevelManager LevelManager)
    {
        manager = unitManager;
        levelManager = LevelManager;

        tempUnitModules = new List<UnitModule>();
        moduleIdLookup = new Dictionary<int, UnitModule>();
    }

    public virtual void StartModuleManager()
    {
        managedModules = tempUnitModules.ToArray();
    }

    public void RegisterUnit(UnitModule unitModule)
    {
        tempUnitModules.Add(unitModule);
        moduleIdLookup.Add(unitModule.InstanceId, unitModule);
    }

    public virtual void SetCommand(CommandData command)
    {

    }

    public virtual void StopCommands(int[] ids)
    {

    }

    public UnitModule GetModuleData(int unitId)
    {
        if (moduleIdLookup.TryGetValue(unitId, out var moduleData))
        {
            return moduleData;
        }

        else
        {
            Debug.LogWarning("Unit id not registered with module: " + this.name);
            return null;
        }
    }
}
