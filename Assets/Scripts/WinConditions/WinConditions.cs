using Sirenix.OdinInspector;
using UnityEngine;

public class WinConditions : MonoBehaviour
{
    [ShowInInspector] protected UnitManager.TeamId winStates;
    public UnitManager.TeamId WinStates
    {
        get { return winStates; }
    }
}
