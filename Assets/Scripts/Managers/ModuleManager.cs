using UnityEngine;
using System.Collections.Generic;

public abstract class ModuleManager : MonoBehaviour
{
    protected bool managerStarted = false;

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

    //These methods InitModuleManager, CustomInitModuleManager and StartModuleManager are run in this specific order.
    //Make sure all important variables are all sorted before StartModuleManager() this method should only included startin the coroutine if applicable.
    public void SetupModuleManager(UnitManager unitManager, LevelManager LevelManager)
    {
        manager = unitManager;
        levelManager = LevelManager;

        tempUnitModules = new List<UnitModule>();
        moduleIdLookup = new Dictionary<int, UnitModule>();
    }

    ///<Summary>
    ///The base must always be executed or managedModules will be null
    ///</Summary>
    public virtual void InitModuleManager()
    {
        managedModules = tempUnitModules.ToArray();
    }

    ///<Summary>
    ///You should only start the managers coroutine in here if applicable
    ///The base needs to be executed or you will be...
    ///</Summary>
    public virtual void StartModuleManager()
    {
        managerStarted = true;
    }

    public void RegisterUnit(UnitModule unitModule)
    {
        tempUnitModules.Add(unitModule);
        moduleIdLookup.Add(unitModule.InstanceId, unitModule);
    }

    public abstract void UnregisterModule(int id);

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
