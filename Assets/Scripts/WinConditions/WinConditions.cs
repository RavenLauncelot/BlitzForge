using Sirenix.OdinInspector;
using UnityEngine;

public abstract class WinConditions : MonoBehaviour
{
    [ShowInInspector] protected UnitManager.TeamId winStates;
    public UnitManager.TeamId WinStates
    {
        get { return winStates; }
    }
}
