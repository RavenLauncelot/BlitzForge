using UnityEngine;

public class RealPlayer : UnitController
{
    public override void StartGame()
    {
        base.StartGame();

        UnitHandler unitHandler = GetComponentInChildren<UnitHandler>();

        unitHandler.InitHandler(manager, controlledTeam);
    }

    public int RemainingUnits()
    {
        int counter = 0;

        foreach (Unit unit in teamUnits)
        {
            if (unit != null)
            {
                counter++;
            }
        }

        return counter;
    }
}
