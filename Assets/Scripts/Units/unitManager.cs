using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Diagnostics.SymbolStore;

public class UnitManager : MonoBehaviour
{
    [SerializeField] private Unit[] units;
    [SerializeField] protected UnitData[] unitData;
    [SerializeField] protected Dictionary<int, int> unitIndexLookup;  //This specifically works with the unitData array. (NOTHING ELSE) IMPORTANTTTT

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

    private void Start()
    {       
        //finding units on map
        unitIndexLookup = new Dictionary<int, int>();
        units = FindObjectsByType<Unit>(FindObjectsSortMode.None);
        unitData = new UnitData[units.Length];

        //I genuinely hate this method so much
        int highestTeamint = 0;
        foreach (Unit unit in units)
        {        
            if ((int)unit.TeamId > highestTeamint)
            {
                highestTeamint = (int)unit.TeamId;
            }
        }    

        //setting up the unitdata struct array
        int i = 0;
        foreach (Unit unit in units)
        {
            unitData[i] = new UnitData
            {
                unitScript = unit,
                observingPos = unit.observingPos,
                detectedTimers = new float[highestTeamint+1],
                teamVisibility = 0,
                teamId = unit.TeamId,
                instanceId = unit.objId,
            };

            unitIndexLookup.Add(unit.objId, i);

            i++;           
        }

        MeshRendManager meshRend = GetComponent<MeshRendManager>();
        meshRend.SetUnitManager(this);

        StartCoroutine(LogicUpdate());
        StartCoroutine(DetectionUpdate());
    }

    private void Update()
    {
        //updating detectionTimers and visibility bitmasks.
        int dataIndex = 0;
        foreach (UnitData data in unitData)
        {
            //while this might an array instead of a dictionary or list. These index still represent the teamId as used in the teamvisivility bitmask

            for (int i = 0; i < data.detectedTimers.Count(); i++)
            {
                if (data.detectedTimers[i] < Time.deltaTime)
                {
                    data.detectedTimers[i] = 0;
                    unitData[dataIndex].teamVisibility &= ~(1u << i);
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
                    int detectedIndex = unitIndexLookup[detected.objId];

                    unitData[detectedIndex].detectedTimers[(int)unitData[u].teamId] = detectionTime;

                    unitData[detectedIndex].teamVisibility |= (1u << (int)unitData[u].teamId);                                
                }

                yield return new WaitForEndOfFrame();
            }
            
            //this is so unity doesn't crash when there are zero units lmoa
            yield return new WaitForEndOfFrame();
        }
    }

    private List<Unit> DetectEnemies(UnitData unit)
    {
        List<Unit> detectedUnits = new List<Unit>();

        Vector3 unitPosition = unit.observingPos.position;
        Collider[] detected = new Collider[20];

        int collisions = Physics.OverlapSphereNonAlloc(unitPosition, unit.unitScript.detectionRange, detected, unitLayermask);
        for (int i = 0; i < collisions; i++)
        {
            //found unit now need to send ray cast to check if not blocked by anything
            if (detected[i].TryGetComponent<Unit>(out Unit unitCode) && unitCode.TeamId != unit.teamId)
            {
                Ray ray = new Ray(unitPosition, unitCode.detectionPos.position - unitPosition);
                RaycastHit hit;
                //Debug.DrawRay(unitPosition, unitCode.transform.position - unitPosition, Color.blue, 2);

                if (Physics.Raycast(ray, out hit))
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
            }            
        }


        return detectedUnits;
    }

    public struct UnitData
    {
        public Unit unitScript;
        public TeamId teamId;
        public uint teamVisibility;
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

    public uint getDebugVisMask(Unit unit)
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
