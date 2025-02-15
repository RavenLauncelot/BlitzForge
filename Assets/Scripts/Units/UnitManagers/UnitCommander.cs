using NUnit.Framework;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using System;

public class UnitCommander : MonoBehaviour
{
    public event EventHandler navTick;
    public event EventHandler commandTick;
    //public event EventHandler enemyDetect;

    public float navCheckTime;
    public float commandCheckTime;

    public enum teamName
    {
        teamA,
        teamB,
        teamC, 
        teamD,
        teamE,
        teamF,
        teamG,
        teamH,
    }

    public new teamName name;

    [SerializeField] private List<Unit> teamUnits;
    [SerializeField] private List<Unit> detectedEnemyUnits;

    private void Start()
    {
        StartCoroutine(navCheckTimer());
        StartCoroutine(commandCheckTimer());
    }

    private IEnumerator navCheckTimer()
    {
        while (true)
        {
            navTick?.Invoke(this, EventArgs.Empty);
            yield return new WaitForSeconds(navCheckTime);
        }
    }

    private IEnumerator commandCheckTimer()
    {
        while (true)
        {
            commandTick?.Invoke(this, EventArgs.Empty);
            yield return new WaitForSeconds(commandCheckTime);
        }
    }
}
