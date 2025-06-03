using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Rendering;

public class UnitManager : MonoBehaviour
{
    [SerializeField] public TeamId managedTeam;

    private LevelManager levelManager;

    //this is a temp list
    [SerializeField] private Unit[] units;

    [SerializeField] public UnitData[] unitData;
    [SerializeField] public Dictionary<int, int> unitIndexLookup;  //Thhis uses a units gameobject id to find the unitData struct associated wiht it 

    [SerializeField] private LayerMask unitLayermask;

    [SerializeField] private TeamId playerTeamId;

    public float detectionTime;

    public enum TeamId
    {
        None,
        PlayerTeam,
        TeamA,
        TeamB,
        TeamC
    }

    //this is temp in the future it will use a scritable object as it's parameters
    //this is ran in awake inside of the levelmanager
    public void InitManager(int TankAmount, GameObject tank, TeamId team, LevelManager levelManager)
    {
        this.levelManager = levelManager;
        managedTeam = team;
        units = new Unit[TankAmount];

        for(int i = 0; i < TankAmount; i++)
        {
            units[i] = Instantiate(tank, transform.position, Quaternion.identity).GetComponent<Unit>();
            units[i].TeamId = managedTeam;
            units[i].unitManager = this;
            units[i].gameObject.name = managedTeam.ToString() + " Tank " + i;
            units[i].initUnit();
        }

        ModuleManager[] modules = GetComponents<ModuleManager>();
        foreach(ModuleManager module in modules)
        {
            module.initModule(this, levelManager);
        }

        //finding units on map
        unitIndexLookup = new Dictionary<int, int>();
        unitData = new UnitData[units.Length];

        //setting up the unitdata struct array
        int u = 0;
        foreach (Unit unit in units)
        {
            //finding the units avaiable modules 
            UnitModule[] unitModules = unit.GetComponents<UnitModule>();
            List<ManagerData> managerData = new List<ManagerData>();

            //This goes through all the unitModules (they should all inherit UnitModule)
            //and then get the manager data which is made from the connected scritable obj to the unit module. 
            foreach (UnitModule module in unitModules)
            {
                managerData.Add(module.GetManagerData());
            }

            unitData[u] = new UnitData
            {
                unitScript = unit,
                observingPos = unit.observingPos,
                aimingPos = unit.aimingPos,
                rayTarget = unit.rayTarget,
                detectedTimers = new float[50],
                teamVisibility = new bool[50],
                teamId = unit.TeamId,
                instanceId = unit.instanceId,

                //This is the list created earlier
                components = managerData
            };

            unitIndexLookup.Add(unit.instanceId, u);

            u++;
        }
    }
    
    private void Start()
    {
        StartCoroutine(LogicUpdate());
        StartCoroutine(DetectionUpdate());
    }

    private void Update()
    {
        //per frame updates for:
        //detectionTimers
        //visibility bitmasks.
        //targetting

        int dataIndex = 0;
        foreach (UnitData data in unitData)
        {
            //this updates all the visibility timers for this team specifically 
            //once a timer reaches zero it will no longer be detected by that team (team enum is represented as a int/index)
            for (int i = 0; i < data.detectedTimers.Count(); i++)
            {
                if (data.detectedTimers[i] < Time.deltaTime)
                {
                    data.detectedTimers[i] = 0;
                    unitData[dataIndex].teamVisibility[i] = false;
                }
                else
                {
                    data.detectedTimers[i] -= Time.deltaTime;
                }
            }

            dataIndex++;
        }
       
    }

    public int[] findUnitsWithModule(ManagerData.ModuleType type)
    {
        List<int> idList = new List<int>();

        //checking through eachs units data
        foreach (UnitData data in unitData)
        {
            //checking through the component list of each unit
            foreach (ManagerData component in data.components)
            {
                if (component.compType == type)
                {
                    idList.Add(data.instanceId);
                }
            }
        }

        return idList.ToArray();
    }

    public ManagerData getCompData(int instanceId, ManagerData.ModuleType type)
    {
        int index = unitIndexLookup[instanceId];

        foreach (ManagerData comp in unitData[index].components)
        {
            if (comp.compType == type)
            {
                return comp;
            }
        }

        return null;
    }

    private IEnumerator LogicUpdate()
    {
        while (true)
        {
            foreach (Unit unit in units)
            {
                if (unit.gameObject.TryGetComponent<ILogicUpdate>(out ILogicUpdate logicUpdate))
                {
                    logicUpdate.TimedLogicUpdate();
                }

                yield return new WaitForEndOfFrame();
            }
        }
    }

    private IEnumerator DetectionUpdate()
    {
        List<Unit> tempList = new List<Unit>();

        while (true)
        {
            for(int u = 0; u < unitData.Count(); u++)             
            {
                tempList = DetectEnemies(unitData[u]);
                
                //now we set the unitdata for the detected enemies to say tehy are detected by the team that detected them
                foreach (Unit detected in tempList)
                {
                    levelManager.setDetected(detected.instanceId, managedTeam, detectionTime);                            
                }

                yield return new WaitForEndOfFrame();
            }
            
            //this is so unity doesn't crash when there are zero units lmoa
            yield return new WaitForEndOfFrame();
        }
    }

    private List<Unit> DetectEnemies(UnitData unitData)
    {      
        List<Unit> detectedUnits = new List<Unit>();

        Vector3 observPos = unitData.observingPos.position;
        Collider[] detected = new Collider[20];

        int collisions = Physics.OverlapSphereNonAlloc(observPos, unitData.unitScript.detectionRange, detected, unitLayermask);
        for (int i = 0; i < collisions; i++)
        {
            //checking if there is a unit script inside the dectected collider
            if (detected[i].TryGetComponent<Unit>(out Unit unitCode))
            {
                //if the team id is the same as the detecting unit it will skip
                if (unitCode.TeamId == unitData.teamId)
                {
                    continue;
                }
            }
            //no unit script skip
            else
            {
                continue;
            }



            //checking unit is within line of sight 
            Ray ray = new Ray(observPos, unitCode.rayTarget.position - observPos);
            RaycastHit hit;
            Debug.DrawRay(observPos, unitCode.rayTarget.position - observPos, Color.blue, 4f);

            if (Physics.Raycast(ray, out hit, unitData.unitScript.detectionRange))
            {
                if (hit.collider.GetComponentInParent<Unit>().instanceId == unitCode.instanceId)
                {
                    detectedUnits.Add(unitCode);
                }

                else
                {
                    //try again
                }
            }
            else
            {
                 Debug.Log("Raycast blocked or didn't reach" + gameObject.name);
            }            
        }       

        return detectedUnits;
    }

    //struct that holds important data about all units in that team
    public struct UnitData
    {
        public Unit unitScript;

        public TeamId teamId;
        public bool[] teamVisibility;
        public float[] detectedTimers;

        public int instanceId;

        //These are positions for raycasts to hit 
        //observing pos is above the tank itself which sends rays to detect tanks
        //aimingpos is the position where shots are fire at the pivot. this sends rays too
        //ray target is where other tanks will send raycasts to 
        public Transform observingPos;
        public Transform aimingPos;
        public Transform rayTarget;

        //These represent the capabilities of each unit 
        //e.g. if they can move shoot detect etc and any specific data
        public List<ManagerData> components;
    }

    //finds all enemy units of that team. This will be removed at some point
    public List<UnitData> GetEnemyUnits(TeamId team)
    {
        List<UnitData> enemyUnits = new List<UnitData>();

        for (int u = 0; u < unitData.Count(); u++)
        {
            if (unitData[u].teamId != team)
            {
                enemyUnits.Add(unitData[u]);
            }         
        }

        return enemyUnits;
    }

    //Debug functions
    public int getDebugTeam(Unit unit)
    {
        return (int)unitData[unitIndexLookup[unit.instanceId]].teamId;
    }

    public bool[] getDebugVisMask(Unit unit)
    {
        return unitData[unitIndexLookup[unit.instanceId]].teamVisibility;
    }

    public float[] getDebugTime(Unit unit)
    {
        return unitData[unitIndexLookup[unit.instanceId]].detectedTimers;
    }

    public string getDebugUnit(Unit unit)
    {
        return unitData[unitIndexLookup[unit.instanceId]].unitScript.gameObject.name;
    }
}
