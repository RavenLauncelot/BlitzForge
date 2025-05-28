using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    //this is temp in the future it will use a scritable object 
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
        }
    }
    
    private void Start()
    {
        //finding units on map
        unitIndexLookup = new Dictionary<int, int>();
        unitData = new UnitData[units.Length];  

        //setting up the unitdata struct array
        int i = 0;
        foreach (Unit unit in units)
        {
            unitData[i] = new UnitData
            {
                unitScript = unit,
                observingPos = unit.observingPos,
                detectedTimers = new float[50],
                teamVisibility = new bool[50],
                teamId = unit.TeamId,
                instanceId = unit.objId,
            };

            unitIndexLookup.Add(unit.objId, i);

            i++;           
        }

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
                    levelManager.setDetected(detected.objId, managedTeam, detectionTime);                            
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

        Vector3 unitPosition = unitData.observingPos.position;
        Collider[] detected = new Collider[20];

        int collisions = Physics.OverlapSphereNonAlloc(unitPosition, unitData.unitScript.detectionRange, detected, unitLayermask);
        for (int i = 0; i < collisions; i++)
        {
            Debug.Log("Units detected within sphere: " + collisions);
            //found unit now need to send ray cast to check if not blocked by anything
            if (detected[i].TryGetComponent<Unit>(out Unit unitCode) && unitCode.TeamId != unitData.teamId)
            {
                Ray ray = new Ray(unitPosition, unitCode.detectionPos.position - unitPosition);
                RaycastHit hit;
                Debug.DrawRay(unitPosition, unitCode.detectionPos.position - unitPosition, Color.blue, 4f);

                if (Physics.Raycast(ray, out hit, unitData.unitScript.detectionRange))
                {
                    if (hit.collider.GetComponentInParent<Unit>() == unitCode)
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
        }       

        return detectedUnits;
    }

    public struct UnitData
    {
        public Unit unitScript;

        public TeamId teamId;
        public bool[] teamVisibility;
        public float[] detectedTimers;

        public int instanceId;

        public Transform observingPos;
        public Transform detectionPos;
    }

    //finds all enemy units of that team
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
        return (int)unitData[unitIndexLookup[unit.objId]].teamId;
    }

    public bool[] getDebugVisMask(Unit unit)
    {
        return unitData[unitIndexLookup[unit.objId]].teamVisibility;
    }

    public float[] getDebugTime(Unit unit)
    {
        return unitData[unitIndexLookup[unit.objId]].detectedTimers;
    }

    public string getDebugUnit(Unit unit)
    {
        return unitData[unitIndexLookup[unit.objId]].unitScript.gameObject.name;
    }
}
