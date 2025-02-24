using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class unitManager : MonoBehaviour
{
    [SerializeField] List<Unit> alliedUnits = new List<Unit>();
    [SerializeField] List<Unit> detectedEnemies = new List<Unit>();

    public bool FindClosestEnemy(Vector3 position, out Unit enemyUnit)
    {
        Unit closestUnit = null;
        Vector3 closestUnitPos;

        if (detectedEnemies == null)
        {
            enemyUnit = null;
            Debug.Log("No detected enemies");
            return false;
        }

        foreach(Unit unit in detectedEnemies)
        {
            if (closestUnit == null)
            {
                closestUnit = unit;
                closestUnitPos = closestUnit.transform.position;
            }

            if (Vector3.Distance(closestUnit.transform.position, position) < Vector3.Distance(unit.transform.position, position))
            {
                closestUnit = unit;
                closestUnitPos = unit.transform.position;
            }
        }

        enemyUnit = closestUnit;
        return true;
    }
}
