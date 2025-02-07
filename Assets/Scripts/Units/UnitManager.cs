using System.Collections;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class UnitManager : MonoBehaviour
{
    //this manages all the units
    //it will check their command completition and pathfinding 
    //it does every certain amount of seconds. This is so units don't check every frame or too often

    public event EventHandler navTick;
    public event EventHandler commandTick;

    public float commandTickTime;
    public float navTickTime;

    public LevelUnitData unitData;

    public bool navTickEnabled;
    public bool cmdTickEnabled;

    IEnumerator commandTicker()
    {
        while (true)
        {
            commandTick?.Invoke(this, EventArgs.Empty);
            yield return new WaitForSeconds(commandTickTime);
        }
    }

    IEnumerator navTicker()
    {
        while (true)
        {
            navTick?.Invoke(this, EventArgs.Empty);
            yield return new WaitForSeconds(navTickTime);
        }
    }

    private void OnEnable()
    {
        StartCoroutine(commandTicker());
        StartCoroutine(navTicker());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public bool checkBool(bool tickBool)
    {
        if (tickBool)
        {
            return true;
        }
        return false;
    }
}
