using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Unity.Collections;
using Unity.VisualScripting;


public class UnitManager : MonoBehaviour
{
    [SerializeField] public TeamId managedTeam;

    private LevelManager levelManager;
    private ModuleManager[] moduleManagers;

    [SerializeReference] public UnitData[] unitData;
    [SerializeField] public Dictionary<int, int> unitDataIndexLookup;  //Thhis uses a units gameobject id to find the unitData struct associated wiht it 

    [SerializeField] private LayerMask unitLayermask;

    public float detectionTime;

    public enum TeamId
    {
        None,
        PlayerTeam,
        TeamA,
        TeamB,
        TeamC
    }

    public void InitManager(SpawnData spawnData, LevelManager levelManager)
    {
        this.levelManager = levelManager;
        managedTeam = spawnData.teamId;

        moduleManagers = GetComponents<ModuleManager>();
        foreach (ModuleManager module in moduleManagers)
        {
            module.InitModule(this, levelManager, managedTeam);
        }

        int unitAmount = 0;
        foreach(int spawnAmount in spawnData.unitAmount)
        {
            unitAmount += spawnAmount;
        }

        //finding units on map
        unitDataIndexLookup = new Dictionary<int, int>();
        unitData = new UnitData[unitAmount];

        //Spawning all the units

        int counter = 0;
        Unit tempUnit = null;
        UnitBlueprint unitBlueprint = null;
        List<ModuleData> moduleData = new List<ModuleData>();
        for(int ub = 0; ub < spawnData.unitBlueprint.Length; ub++)
        {
            unitBlueprint = spawnData.unitBlueprint[ub];
            moduleData = unitBlueprint.moduleData;

            for (int i = 0; i < spawnData.unitAmount[ub]; i++)
            {
                tempUnit = Instantiate(unitBlueprint.unitPrefab, transform.position, Quaternion.identity).GetComponent<Unit>();
                tempUnit.TeamId = managedTeam;
                //tempUnit.unitManager = this;
                tempUnit.gameObject.name = managedTeam.ToString() + " Tank " + counter;
                tempUnit.InitUnit();

                List<ModuleData> modulesData = new List<ModuleData>();
                foreach (ModuleData moduleDataScriptable in moduleData)
                {
                    modulesData.Add(moduleDataScriptable.Clone());
                }

                unitData[counter] = new UnitData
                {
                    unitScript = tempUnit,
                    observingPos = tempUnit.observingPos,
                    aimingPos = tempUnit.aimingPos,
                    rayTarget = tempUnit.rayTarget,
                    teamVisibility = new bool[8],
                    teamId = tempUnit.TeamId,
                    instanceId = tempUnit.InstanceId,

                    health = unitBlueprint.unitData.health,
                    isAlive = true,

                    components = modulesData
                };

                unitDataIndexLookup.Add(tempUnit.InstanceId, counter);
                counter++;
            }
        }
    }

    private void Start()
    {
        
    }

    public int[] GetIdsWithModule(string moduleType)
    {
        List<int> idList = new List<int>();

        //checking through eachs units data
        foreach (UnitData data in unitData)
        {
            //checking through the component list of each unit
            foreach (ModuleData component in data.components)
            {
                if (component.moduleType == moduleType)
                {
                    idList.Add(data.instanceId);
                }
            }
        }

        return idList.ToArray();
    }

    public ModuleData GetModuleData(int instanceId, string type)
    {
        if (unitDataIndexLookup.TryGetValue(instanceId, out int index))
        {
            foreach (ModuleData comp in unitData[index].components)
            {
                if (comp.moduleType == type)
                {
                    return comp;
                }
            }
        }

        return null;
    }

    public bool TryGetModuleData(int instanceId, string moduleType, out ModuleData result)
    {
        if (unitDataIndexLookup.TryGetValue(instanceId, out int index))
        {
            foreach (ModuleData comp in unitData[index].components)
            {
                if (comp.moduleType == moduleType)
                {
                    result = comp;
                    return true;
                }
            }
        }

        result = null; 
        return false;
    }

    //private IEnumerator DetectionUpdate()
    //{
    //    List<Unit> tempList = new List<Unit>();

    //    while (true)
    //    {
    //        for(int u = 0; u < unitData.Count(); u++)             
    //        {
    //            tempList = DetectEnemies(unitData[u]);
                
    //            //now we set the unitdata for the detected enemies to say tehy are detected by the team that detected them
    //            foreach (Unit detected in tempList)
    //            {
    //                levelManager.setDetected(detected.instanceId, managedTeam, detectionTime);                            
    //            }

    //            yield return new WaitForEndOfFrame();
    //        }
            
    //        //this is so unity doesn't crash when there are zero units lmoa
    //        yield return new WaitForEndOfFrame();
    //    }
    //}

    //private List<Unit> DetectEnemies(UnitData unitData)
    //{      
    //    List<Unit> detectedUnits = new List<Unit>();

    //    Vector3 observPos = unitData.observingPos.position;
    //    Collider[] detected = new Collider[20];

    //    int collisions = Physics.OverlapSphereNonAlloc(observPos, unitData.unitScript.detectionRange, detected, unitLayermask);
    //    for (int i = 0; i < collisions; i++)
    //    {
    //        //checking if there is a unit script inside the dectected collider
    //        if (detected[i].TryGetComponent<Unit>(out Unit unitCode))
    //        {
    //            //if the team id is the same as the detecting unit it will skip
    //            if (unitCode.TeamId == unitData.teamId)
    //            {
    //                continue;
    //            }
    //        }
    //        //no unit script skip
    //        else
    //        {
    //            continue;
    //        }



    //        //checking unit is within line of sight 
    //        Ray ray = new Ray(observPos, unitCode.rayTarget.position - observPos);
    //        RaycastHit hit;
    //        Debug.DrawRay(observPos, unitCode.rayTarget.position - observPos, Color.blue, 4f);

    //        if (Physics.Raycast(ray, out hit, unitData.unitScript.detectionRange))
    //        {
    //            if (hit.collider.GetComponentInParent<Unit>().instanceId == unitCode.instanceId)
    //            {
    //                detectedUnits.Add(unitCode);
    //            }

    //            else
    //            {
    //                //try again
    //            }
    //        }
    //        else
    //        {
    //             Debug.Log("Raycast blocked or didn't reach" + gameObject.name);
    //        }            
    //    }       

    //    return detectedUnits;
    //}

    //struct that holds important data about all units in that team
    
    public struct UnitData
    {
        public int instanceId;
        public bool isAlive;

        public Unit unitScript;

        public TeamId teamId;
        public bool[] teamVisibility;

        public float health;

        //These are positions for raycasts to hit 
        //observing pos is above the tank itself which sends rays to detect tanks
        //aimingpos is the position where shots are fire at the pivot. this sends rays too
        //ray target is where other tanks will send raycasts to 
        public Transform observingPos;
        public Transform aimingPos;
        public Transform rayTarget;

        //These represent the capabilities of each unit 
        //e.g. if they can move shoot detect etc and any specific data
        [SerializeReference] public List<ModuleData> components;
    }
    
    public int[] GetDetectedUnitsIds(TeamId detectedBy)
    {
        List<int> enemyUnits = new List<int>();

        foreach(UnitData unit in unitData)
        {
            if (unit.teamVisibility[(int)detectedBy] == true)
            {
                enemyUnits.Add(unit.instanceId);
            }
        }

        if (enemyUnits.Count == 0)
        {
            return null;
        }
        else
        {
            return enemyUnits.ToArray();
        }
    }

    public void DestroyUnit(int instanceId)
    {
        if (unitDataIndexLookup.TryGetValue(instanceId, out int index))
        {
            Debug.Log("Unit destroyed: " + unitData[index].unitScript.gameObject.name);

            //unitData[index].unitScript  do destroy animation or remove object etc.
            //unitData[index].alive = false;
            //unitData[index].instanceId = 0;
            unitDataIndexLookup.Remove(instanceId);

            unitData = unitData.Where(val => val.instanceId != instanceId).ToArray();

            foreach(ModuleManager moduleManager in moduleManagers)
            {
                moduleManager.RemoveId(instanceId);
            }
        }
    }

    //This takes a generic command and depending on the enum type of module it will send it to that module
    public void SendCommand(CommandData command)
    {
        foreach(ModuleManager moduleManager in moduleManagers)
        {
            moduleManager.StopCommands(command.selectedUnits);
            if (command.targetModule == moduleManager.ModuleType)
            {
                moduleManager.SetCommand(command);
            }
        }
    }

    public UnitData GetUnitDataReadOnly(int instanceId)
    {
        int unitDataIndex = unitDataIndexLookup[instanceId];

        return unitData[unitDataIndex];
    }

    private ModuleManager GetModuleManager(string type)
    {
        foreach(ModuleManager manager in moduleManagers)
        {
            if (manager.ModuleType == type)
            {
                return manager;
            }       
        }

        Debug.Log("Module Type, " + type + " is not attached");
        return null;
    }

}
