using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using static UnitManager;

public class LevelManager : MonoBehaviour
{
    //This loads the entire level. Can set the amount of teams and certain Data 
    //this is only used once upon loading 

    [SerializeReference]
    public List<SpawnData> spawnData;
    public List<Transform> spawnLocations;

    public GameObject unitManagerPrefab;
    public UnitBlueprint[] unitTypes;

    public UnitManager[] unitManagers;

    //this will spawn teams and unit managers
    public void Awake()
    {
        unitManagers = new UnitManager[spawnData.Count];

        for(int i  = 0; i < spawnData.Count; i++)
        {
            //spawning unitManager
            GameObject unitMan = Instantiate(unitManagerPrefab, spawnLocations[i].position, Quaternion.identity);
            unitMan.GetComponent<UnitManager>().InitManager(spawnData[i], this);
            unitMan.gameObject.name = spawnData[i].teamId.ToString() + " Manager";

            //Adding unitManager to unitManager array 
            unitManagers[i] = unitMan.GetComponent<UnitManager>();
        }
    }

    //this will loop through all unitManagers and find enemy units of the team inputed
    public List<UnitData> getEnemyUnits(UnitManager.TeamId team)
    {
        List<UnitData> enemyUnits = new List<UnitData>();

        for (int i = 0; i <= unitManagers.Length; i++)
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

    public UnitData getUnitData(int instanceId)
    {
        if (instanceId == 0)
        {
            return new UnitData
            {
                instanceId = 0
            };
        }

        foreach(UnitManager unitManager in unitManagers)
        {
            if (unitManager.unitDataIndexLookup.TryGetValue(instanceId, out var unitDataIndex))
            {
                UnitData unitData = unitManager.unitData[unitDataIndex];
                return unitData;
            }
        }

        return new UnitData
        {         
            instanceId = 0
        };
    }

    public void setDetected(int instanceId, UnitManager.TeamId detectedBy, float detectionTime)
    {
        for (int i = 0; i <= spawnData.Count; i++)
        {
            if (unitManagers[i].unitDataIndexLookup.TryGetValue(instanceId, out int index))
            {
                unitManagers[i].unitData[index].teamVisibility[(int)detectedBy] = true;
                unitManagers[i].unitData[index].detectedTimers[(int)detectedBy] = detectionTime;
                //Debug.Log("Set unit as detected - detected by team " + detectedBy);
                return;
            }
        }
    }
}
