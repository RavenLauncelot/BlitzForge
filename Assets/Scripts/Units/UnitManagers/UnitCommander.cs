using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class UnitCommander : MonoBehaviour
{
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
}
