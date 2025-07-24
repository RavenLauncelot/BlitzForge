using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public abstract class UnitController : MonoBehaviour
{
    [SerializeField] protected UnitManager.TeamId controlledTeam;
    [SerializeField] protected Unit[] teamUnits;
    protected UnitManager manager;

    protected bool gameStarted = false;

    public void InitController(List<Unit> units, UnitManager managerIn, UnitManager.TeamId controlledTeamIn)
    {
        teamUnits = units.ToArray();
        manager = managerIn;
        controlledTeam = controlledTeamIn;
    }

    public virtual void StartGame()
    {
        if (gameStarted)
        {
            return;
        }

        gameStarted = true;
    }
}
