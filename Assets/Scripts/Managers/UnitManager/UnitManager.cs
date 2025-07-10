using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    //[SerializeField] public TeamId managedTeam;

    private LevelManager levelManager;
    private Dictionary<string, ModuleManager> moduleManagers;

    [SerializeField] private LayerMask unitLayermask;

    public enum TeamId
    {
        None,
        PlayerTeam,
        TeamA,
        TeamB,
        TeamC,
        TeamD,
        TeamE,
        TeamF
    }

    public void InitManager(List<SpawnData> spawnData, LevelManager levelManagerIn)
    {
        levelManager = levelManagerIn;
        moduleManagers = new Dictionary<string, ModuleManager>();

        //Finding all the unitModules and adding them to a dictionary and initiliasing them
        ModuleManager[] modules = GetComponents<ModuleManager>();
        foreach (ModuleManager module in modules)
        {
            moduleManagers.Add(module.ManagerType, module);
            module.InitModule(this, levelManager);
        }

        //spawning all units for all teams 
        foreach(SpawnData data in spawnData)
        {
            SpawnTeam(data);
        }

        //new version
        foreach (ModuleManager module in moduleManagers.Values)
        {
            module.StartModuleManager();
        }
    }

    private void SpawnTeam(SpawnData spawnData)
    {
        //Spawning all the units for this team
        int counter = 0;
        Unit tempUnit = null;
        TeamId team = spawnData.teamId;
        //Loop for the amount of spawns - A spawn contains the unit to be spawned, location and the amount to be spawned.
        foreach (Spawn spawn in spawnData.spawns)
        {
            //Spawning the individual units
            for (int i = 0; i < spawn.amountOf; i++)
            {
                tempUnit = Instantiate(spawn.objectToBeSpawned, spawn.spawnPoint.position, Quaternion.identity).GetComponent<Unit>();
                tempUnit.InitUnit(team);
                tempUnit.name = spawnData.teamId.ToString() + " Tank " + counter;

                //Getting the modules connected to the unit and registering them with the respective manager.
                UnitModule[] unitModules = tempUnit.GetModules();
                foreach (UnitModule module in unitModules)
                {
                    module.InitModule(tempUnit.InstanceId, spawnData.teamId); //Assigning the instance id to the module so it can be found later
                    module.CustomInit();
                    RegisterModule(module);
                }

                counter++;
            }
        }
    }

    private void RegisterModule(UnitModule unitModule)
    {
        foreach (string targetManager in unitModule.TargetModuleManager)
        {
            if (moduleManagers.TryGetValue(targetManager, out var module))
            {
                module.RegisterUnit(unitModule);
            }
        }
    }

    //This needs to be done 
    public void DestroyUnit(int instanceId)
    {
        
    }

    public void DamageUnit(int instanceId, float damage)
    {

    }

    public bool IsTargetDetected(int targetId, TeamId attackingTeam)
    {
        VisibilityModule visibilityModule = GetUnitModule<VisibilityManager>(targetId) as VisibilityModule;

        if (visibilityModule.visibilityMask[(int)attackingTeam] == true)
        {
            return true;
        }

        return false;
    }

    public UnitModule GetUnitModule<TManager>(int instanceId)
        where TManager : ModuleManager
    {
        foreach(var moduleManager in moduleManagers.Values)
        {
            if (moduleManager is TManager typedManager)
            {
                UnitModule unitModule = moduleManager.GetModuleData(instanceId);
                return unitModule;
            }
        }

        return null;
    }

    //This takes a generic command and depending on the enum type of module it will send it to that module
    public void SendCommand(CommandData command)
    {
        foreach(ModuleManager moduleManager in moduleManagers.Values)
        {
            moduleManager.StopCommands(command.selectedUnits);
            if (command.targetModule == moduleManager.ManagerType)
            {
                moduleManager.SetCommand(command);
            }
        }
    }
}
