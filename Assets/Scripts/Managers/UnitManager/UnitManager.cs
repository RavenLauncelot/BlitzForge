using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    //[SerializeField] public TeamId managedTeam;

    private LevelManager levelManager;
    private Dictionary<ModuleType, ModuleManager> moduleManagers;

    [SerializeField] private GameObject playerControllerPrefab;
    [SerializeField] private GameObject aiControllerPrefab;

    [SerializeField] private LayerMask unitLayermask;

    Unit[] allUnits;
    Dictionary<int, Unit> unitIdLookUp;

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
        //temp list for unit controllers so i can start them once everything is spawned in 
        List<UnitController> unitControllers = new List<UnitController>();

        List<Unit> tempAllUnits = new List<Unit>();

        levelManager = levelManagerIn;
        moduleManagers = new Dictionary<ModuleType, ModuleManager>();

        //Finding all the unitModules and adding them to a dictionary and initiliasing them
        ModuleManager[] modules = GetComponents<ModuleManager>();
        foreach (ModuleManager module in modules)
        {
            moduleManagers.Add(module.ManagerType, module);
            module.SetupModuleManager(this, levelManager);
        }

        //spawning units per team.
        foreach (SpawnData data in spawnData)
        {
            UnitController controller;
            //Spawn unit controller. Player or AI. The player just spawns a UnitHandler. I will include it with the camera as it needs that and a additional script
            if (data.AiPlayer)
            {
                controller = Instantiate(aiControllerPrefab).GetComponent<UnitController>();
            }
            else
            {
                controller = Instantiate(playerControllerPrefab).GetComponent<UnitController>();
            }

            List<Unit> teamUnits = SpawnTeam(data);
            controller.InitController(teamUnits, this, data.teamId);
            unitControllers.Add(controller);

            tempAllUnits.AddRange(teamUnits);
            teamUnits.Clear();
        }

        allUnits = tempAllUnits.ToArray();
        unitIdLookUp = allUnits.ToDictionary(val => val.InstanceId);

        //This is a bit weird
        //But each module needs to cast their array of UnitModules to their respective module they are managing
        //While this could be done in StartModuleManager() it can cause issues if other things access modules that access this custom array 
        //So they're in total there is a InitModuleManager() which is done first equivalent of Start(). Then CustomInit() and then StartModuleManager which then starts then loop. 
        //Diciontionaries man. Beg you just like instantiate before runtime
        foreach (ModuleManager moduleManager in moduleManagers.Values)
        {
            moduleManager.InitModuleManager();
        }

        //new version
        foreach (ModuleManager moduleManager in moduleManagers.Values)
        {
            moduleManager.StartModuleManager();
        }


        //Once is everything is initiliased start the controllers
        foreach (UnitController controller in unitControllers)
        {
            controller.StartGame();
        }
    }

    private List<Unit> SpawnTeam(SpawnData spawnData)
    {
        List<Unit> teamUnits = new List<Unit>();

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

                teamUnits.Add(tempUnit);

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
        


        return teamUnits;
    }

    private void RegisterModule(UnitModule unitModule)
    {
        foreach (ModuleType targetManager in unitModule.TargetModuleManager)
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

    public Unit[] GetDetectedUnits(TeamId detectedBy)
    {
        VisibilityManager visManager;
        moduleManagers.TryGetValue(ModuleType.VisManager, out ModuleManager module);
        visManager = module as VisibilityManager;

        List<Unit> detectedUnits = new List<Unit>();       

        foreach (VisibilityModule visModule in visManager.visibilityModules)
        { 
            //visibilityModule = GetUnitManagerModule<VisibilityManager>(unit.InstanceId) as VisibilityModule;
            //Debug.Log("Vis module: " + visibilityModule.visibilityMask[0]);

            //since units don't detected themselves I don't need to check if they are on the same team
            if (visModule.visibilityMask[(int)detectedBy] == true)
            {
                detectedUnits.Add(unitIdLookUp.GetValueOrDefault(visModule.InstanceId));
            }
        }

        return detectedUnits.ToArray();
    }

    public bool IsTargetDetected(int targetId, TeamId detectedBy)
    {
        VisibilityManager visibilityManager = moduleManagers.GetValueOrDefault(ModuleType.VisManager) as VisibilityManager;

        VisibilityModule visModule;
        if (visibilityManager.visModuleIdLookup.TryGetValue(targetId, out visModule))
        { 
            //This check if the timer is above 1.
            //When it would check the mask it would result in the units flickering. 
            //as the timers would not be refreshed. 
            //I was going to refresh the timers in here but that could potentially mean units that are not within LOS would stay detected.
            if (visModule.visibilityTimers[(int)detectedBy] > 1f)
            {               
                return true;           
            }
        }

        return false;
    }

    public UnitModule GetUnitManagerModule<TManager>(int instanceId)
        where TManager : ModuleManager
    {
        foreach (var moduleManager in moduleManagers.Values)
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
        foreach (ModuleManager moduleManager in moduleManagers.Values)
        {
            moduleManager.StopCommands(command.selectedUnits);
            if (command.targetModule == moduleManager.ManagerType)
            {
                moduleManager.SetCommand(command);
            }
        }
    }
}