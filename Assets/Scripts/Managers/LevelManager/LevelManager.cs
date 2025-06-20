using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using static UnitManager;
using UnityEngine.Rendering;
using System.Linq;
using System.Security.Cryptography;

public class LevelManager : MonoBehaviour
{
    //This loads the entire level. Can set the amount of teams and certain Data 

    [SerializeReference]
    public List<SpawnData> spawnData;
    public List<Transform> spawnLocations;

    public GameObject unitManagerPrefab;
    public UnitBlueprint[] unitTypes;

    public UnitManager[] unitManagers;
    public Dictionary<int,UnitManager> unitManDictionary;

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

    public UnitData GetUnitData(int instanceId)
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

    public bool TryGetUnitData(int instanceId, out UnitData unitData)
    {
        if (instanceId == 0)
        {
            unitData = new UnitData();
            return false;           
        }

        foreach (UnitManager unitManager in unitManagers)
        {
            if (unitManager.unitDataIndexLookup.TryGetValue(instanceId, out var unitDataIndex))
            {
                unitData = unitManager.unitData[unitDataIndex];
                return true;
            }
        }

        unitData = new UnitData();
        return false;
    }

    public bool IsUnitDetected(int instanceId, UnitManager.TeamId detectedBy)
    {
        if (GetUnitData(instanceId).teamVisibility[(int)detectedBy] == true)
        {
            return true;
        }

        return false;
    }

    public bool IsValidTarget(int targetId, UnitManager.TeamId attackingTeam)
    {
        if (TryGetUnitData(targetId , out var unitData))
        {
            if (unitData.teamVisibility[(int)attackingTeam] == true && unitData.isAlive == true)
            {
                return true;
            }
        }

        return false;
    }

    public void SetDetected(int instanceId, UnitManager.TeamId detectedBy, float detectionTime)
    {
        foreach (UnitManager unitManager in unitManagers)
        {
            if (unitManager.unitDataIndexLookup.TryGetValue(instanceId, out int index))
            {
                unitManager.unitData[index].teamVisibility[(int)detectedBy] = true;

                DetectionData data = unitManager.GetModuleData(instanceId, "DetectionManager") as DetectionData;
                data.detectedTimers[(int)detectedBy] = detectionTime;

                return;
            }
        }

        return;
    }

    public int[] GetDetectedEnemies(UnitManager.TeamId detectedBy)
    {
        int[] detectedEnemies = new int[0];

        foreach (UnitManager unitManager in unitManagers)
        {
            int[] detectedTeamUnits = unitManager.GetDetectedUnitsIds(detectedBy);

            if (detectedTeamUnits == null)
            {
                continue;
            }

            detectedEnemies.Concat(detectedTeamUnits);

            Debug.Log(detectedEnemies[0]);
        }

        return detectedEnemies;
    }

    public void DamageUnit(int instanceId, float damage)
    {
        foreach (UnitManager unitManager in unitManagers)
        {
            if (unitManager.unitDataIndexLookup.TryGetValue(instanceId, out int index)) 
            {
                unitManager.unitData[index].health -= damage;

                Debug.Log("Damage dealt - " + damage);

                if (unitManager.unitData[index].health <= 0)
                {
                    unitManager.DestroyUnit(instanceId);
                }

                return;
            }
        }
    }

    private ModuleManager GetManager(int instanceId)
    {
        return null;
    }
}
