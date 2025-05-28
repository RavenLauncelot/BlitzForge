using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using static UnitManager;

public class LevelManager : MonoBehaviour
{
    //This loads the entire level. Can set the amount of teams and certain Data 
    //this is only used once upon loading 

    public List<UnitManager.TeamId> teams;
    public UnitManager[] unitManagers;

    public int tankPerTeam;

    public List<Transform> spawnLocations;

    //this is temporary for now
    public GameObject unitManager;
    public GameObject tank;

    //this will spawn teams and unit managers
    public void Awake()
    {
        unitManagers = new UnitManager[teams.Count];

        for(int i  = 0; i < teams.Count; i++)
        {
            //spawning unitManager
            GameObject unitMan = Instantiate(unitManager, spawnLocations[i].position, Quaternion.identity);
            unitMan.GetComponent<UnitManager>().InitManager(tankPerTeam, tank, teams[i], this);
            unitMan.gameObject.name = teams[i].ToString() + "Manager";

            //Adding unitManager to unitManager array 
            unitManagers[i] = unitMan.GetComponent<UnitManager>();
        }
    }

    //this will loop through all unitManagers and find enemy units of the team inputed
    public List<UnitData> getEnemyUnits(UnitManager.TeamId team)
    {
        List<UnitData> enemyUnits = new List<UnitData>();

        for (int i = 0; i <= teams.Count; i++)
        {   
            for (int u = 0; u < unitManagers[i].unitData.Length; u++)
            {
                if (unitManagers[i].unitData[u].teamId != team)
                {
                    enemyUnits.Add(unitManagers[i].unitData[u]);
                }
            }     
        }

        return enemyUnits;
    }

    public void setDetected(int instanceId, UnitManager.TeamId detectedBy, float detectionTime)
    {
        Debug.Log("Unit was detected potentially");
        for (int i = 0; i <= teams.Count; i++)
        {
            if (unitManagers[i].unitIndexLookup.TryGetValue(instanceId, out int index))
            {
                unitManagers[i].unitData[index].teamVisibility[(int)detectedBy] = true;
                unitManagers[i].unitData[index].detectedTimers[(int)detectedBy] = detectionTime;
                Debug.Log("Set unit as detected");
                return;
            }
        }
    }
}
