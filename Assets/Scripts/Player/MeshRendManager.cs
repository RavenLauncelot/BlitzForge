using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class MeshRendManager : MonoBehaviour
{
    [SerializeField] public UnitManager.TeamId managedTeam;
    public LevelManager levelManager;
    UnitManager[] unitManagers;

    int[] detectedUnits = new int[0];

    private void Start()
    {
        unitManagers = levelManager.unitManagers;
    }

    // Update is called once per frame
    public void Update()
    {
        foreach(var manager in unitManagers)
        {
            if (manager.managedTeam == managedTeam)
            {
                continue;
            }

            foreach(UnitManager.UnitData unit in manager.unitData)
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
}
