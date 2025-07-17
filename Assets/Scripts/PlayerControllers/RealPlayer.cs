using UnityEngine;

public class RealPlayer : UnitController
{
    public override void StartGame()
    {
        base.StartGame();

        UnitHandler unitHandler = GetComponentInChildren<UnitHandler>();

        unitHandler.InitHandler(manager, controlledTeam);
    }
}
