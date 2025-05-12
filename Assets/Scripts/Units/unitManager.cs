using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using Unity.VisualScripting;
using Unity.Android.Gradle.Manifest;
using System.Linq;

public class unitManager : MonoBehaviour
{
    [SerializeField] private Unit[] units;
    [SerializeField] private UnitData[] unitData;
    [SerializeField] private Dictionary<Unit, int> unitIndexLookup;  //This specifically works with the unitData array. (NOTHING ELSE) IMPORTANTTTT

    [SerializeField] private LayerMask unitLayermask;

    [SerializeField] private TeamId playerTeamId;

    public float detectionTime;

    //[SerializeField] private List<Unit> playerVisibleUnits;

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
        unitIndexLookup = new Dictionary<Unit, int>();
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



        int i = 0;
        foreach (Unit unit in units)
        {
            unitData[i] = new UnitData
            {
                unitScript = units[i],
                detectedTimers = new float[highestTeamint+1],
                teamVisibility = 0,
                teamId = unit.TeamId
            };

            unitIndexLookup.Add(units[i], i);

            i++;           
        }



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
            foreach (UnitData data in unitData)
            {
                tempList = DetectEnemies(data);
                
                //now we set the unitdata for the detected enemies to say tehy are detected by the team that detected them
                foreach (Unit detected in tempList)
                {
                    UnitData unitDataRef = findUnitData(detected);

                    unitDataRef.detectedTimers[(int)data.teamId] = detectionTime;

                    unitDataRef.teamVisibility |= (1u << (int)data.teamId);                                
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

        Vector3 UnitPosition = unit.unitScript.transform.position;
        Collider[] detected = new Collider[20];

        int collisions = Physics.OverlapSphereNonAlloc(UnitPosition, unit.unitScript.detectionRange, detected, unitLayermask);
        for (int i = 0; i < collisions; i++)
        {
            if (detected[i].TryGetComponent<Unit>(out Unit unitCode))
            {               
                if (unit.teamId != unitCode.TeamId)
                {
                    detectedUnits.Add(unitCode);                   
                }
            }            
        }

        return detectedUnits;
    }

    private struct UnitData
    {
        public Unit unitScript;
        public TeamId teamId;
        public uint teamVisibility;
        public float[] detectedTimers;
    }

    private UnitData findUnitData(Unit unit)
    {
        return unitData[unitIndexLookup[unit]];
    }





    //Debug functions
    public int getDebugTeam(Unit unit)
    {
        return (int)unitData[unitIndexLookup[unit]].teamId;
    }

    public uint getDebugVisMask(Unit unit)
    {
        return unitData[unitIndexLookup[unit]].teamVisibility;
    }

    public float[] getDebugTime(Unit unit)
    {
        return unitData[unitIndexLookup[unit]].detectedTimers;
    }

    public string getDebugUnit(Unit unit)
    {
        return unitData[unitIndexLookup[unit]].unitScript.gameObject.name;
    }
}
