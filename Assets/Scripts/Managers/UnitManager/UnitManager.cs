using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    private LevelManager levelManager;
    private Dictionary<ModuleType, ModuleManager> moduleManagers;

    private UnitController[] unitControllers;

    [SerializeField] private GameObject playerControllerPrefab;
    [SerializeField] private GameObject aiControllerPrefab;

    [SerializeField] private LayerMask unitLayermask;

    Unit[] allUnits;
    public Dictionary<int, Unit> unitIdLookUp;

    private bool gameStarted = false;

    //public enum TeamId
    //{
    //    None,
    //    PlayerTeam,
    //    TeamA,
    //    TeamB,
    //    TeamC,
    //    TeamD,
    //    TeamE,
    //    TeamF
    //}

    public void InitManager(List<SpawnData> spawnData, LevelManager levelManagerIn)
    {
        //temp list for unit controllers so i can start them once everything is spawned in 
        List<UnitController> unitControllersList = new List<UnitController>();

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
            unitControllersList.Add(controller);

            tempAllUnits.AddRange(teamUnits);
            teamUnits.Clear();
        }

        unitControllers = unitControllersList.ToArray();
        allUnits = tempAllUnits.ToArray();
        unitIdLookUp = allUnits.ToDictionary(val => val.InstanceId);

        //This is a bit weird
        //But some moduled needs to cast their array of UnitModules to their respective module they are managing
        //While this could be done in StartModuleManager() it can cause issues if other things access modules that access this custom array 
        //So they're in total there is a InitModuleManager() which is done first equivalent of Start(). Then CustomInit() and then StartModuleManager which then starts then loop. 
        //Diciontionaries man. Beg you just like instantiate before runtime
        foreach (ModuleManager moduleManager in moduleManagers.Values)
        {
            moduleManager.InitModuleManager();
        }   
    }

    public void StartGame()
    {
        if (gameStarted == false)
        {
            //new version
            foreach (ModuleManager moduleManager in moduleManagers.Values)
            {
                moduleManager.StartModuleManager();
            }

            foreach (UnitController controller in unitControllers)
            {
                controller.StartGame();
            }
        }
    }

    private List<Unit> SpawnTeam(SpawnData spawnData)
    {
        List<Unit> teamUnits = new List<Unit>();

        //Spawning all the units for this team
        int counter = 0;
        Unit tempUnit = null;
        TeamInfo.TeamId team = spawnData.teamId;
        //Loop for the amount of spawns - A spawn contains the unit to be spawned, location and the amount to be spawned.
        foreach (Spawn spawn in spawnData.spawns)
        {
            Vector3[] positionsToSpawn = NavmeshTools.EvenSpacing(spawn.amountOf, spawn.spawnPoint.position, 10f);

            //Spawning the individual units
            for (int i = 0; i < spawn.amountOf; i++)
            {
                tempUnit = Instantiate(spawn.objectToBeSpawned, positionsToSpawn[i], Quaternion.identity).GetComponent<Unit>();
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
        if (moduleManagers.TryGetValue(unitModule.TargetModuleManager, out var module))
        {
            module.RegisterUnit(unitModule);
        }        
    }

 
    public void DestroyUnit(Unit unit)
    {      
        foreach (ModuleManager manager in moduleManagers.Values)
        {
            manager.StopCommands(new int[] {unit.InstanceId});
            manager.UnregisterModule(unit.InstanceId);
        }

        unitIdLookUp.Remove(unit.InstanceId);
        allUnits = allUnits.Where(val => val != unit).ToArray();
        unit.DestroyUnit();
    }

    public void DamageUnit(int instanceId, float damage)
    {
        if (unitIdLookUp.TryGetValue(instanceId, out Unit unit))
        {
            unit.DamageUnit(damage);

            if (unit.Health <= 0)
            {
                DestroyUnit(unit);
            }
        }
    }

    public Unit[] GetDetectedUnits(TeamInfo.TeamId detectedBy)
    {
        VisibilityManager visManager;
        visManager = VisibilityManager.instance;

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