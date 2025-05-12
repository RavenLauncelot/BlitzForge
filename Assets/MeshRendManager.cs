using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class MeshRendManager : MonoBehaviour
{
    [SerializeField] private UnitManager.TeamId managedTeam;
    private UnitManager unitManager;

    List<UnitManager.UnitData> detectedUnits;

    public void SetUnitManager(UnitManager manager)
    {
        unitManager = manager;
    }

    // Update is called once per frame
    void Update()
    {
        detectedUnits = unitManager.GetEnemyUnits(managedTeam);

        foreach (UnitManager.UnitData unit in detectedUnits)
        {
            if ((unit.teamVisibility & (1u << (int)managedTeam)) != 0)
            {
                unit.unitScript.GetComponentInChildren<MeshRenderer>().enabled = true;
            }
            else
            {
                unit.unitScript.GetComponentInChildren<MeshRenderer>().enabled = false;
            }
        }
    }
}
