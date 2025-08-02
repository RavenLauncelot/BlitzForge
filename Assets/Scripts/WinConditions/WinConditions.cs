using Sirenix.OdinInspector;
using UnityEngine;

public abstract class WinConditions : MonoBehaviour
{
    [ShowInInspector] protected TeamInfo.TeamId winStates;
    public TeamInfo.TeamId WinStates
    {
        get { return winStates; }
    }
}
