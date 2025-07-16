using NUnit.Framework;
using Sirenix.OdinInspector;
using UnityEngine;
using System.Collections.Generic;

public class PlayerScript : MonoBehaviour
{
    Unit[] unitArray;
    UnitManager unitManager;

    public void InitController(UnitManager unitManagerIn, UnitManager.TeamId team, List<Unit> teamUnits)
    {
        unitManager = unitManagerIn;

        unitArray = teamUnits.ToArray();
    }


}


