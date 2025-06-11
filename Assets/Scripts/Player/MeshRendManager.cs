using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class MeshRendManager : MonoBehaviour
{
    [SerializeField] public UnitManager.TeamId managedTeam;
    public UnitManager unitManager;

    List<UnitManager.UnitData> detectedUnits;

    // Update is called once per frame
    void Update()
    {
        detectedUnits = unitManager.GetEnemyUnits(managedTeam);

        foreach (UnitManager.UnitData unit in detectedUnits)
        {
            if (unit.teamVisibility[(int)managedTeam] == true)
            {
                unit.unitScript.MeshRendEnabled(true); 
            }
            else
            {
                unit.unitScript.MeshRendEnabled(false);
            }
        }
    }
}
