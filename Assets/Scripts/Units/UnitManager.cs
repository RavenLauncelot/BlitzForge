using System.Collections;
using UnityEngine;
using System;

public class UnitManager : MonoBehaviour
{
    //this manages all the units
    //it will check their command completition and pathfinding 
    //it does every certain amount of seconds. This is so units don't check every frame or too often

    public event EventHandler navTick;
    public event EventHandler commandTick;

    public float commandTickTime;
    public float navTickTime;

    IEnumerator commandTicker()
    {
        commandTick?.Invoke(this, EventArgs.Empty);
        yield return new WaitForSeconds(commandTickTime);
    }

    IEnumerator navTicker()
    {
        navTick?.Invoke(this, EventArgs.Empty);
        yield return new WaitForSeconds(navTickTime);
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
}
