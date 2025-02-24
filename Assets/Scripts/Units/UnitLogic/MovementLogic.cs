using UnityEngine;

public class MovementLogic : CmdLogic
{
    [SerializeField] private UnitMovement movementComp;

    private void Start()
    {
        command = CmdLogic.commandType.MovementCmd;
        Unit = GetComponent<Unit>();

        if (TryGetComponent<UnitMovement>(out UnitMovement movementComponent))
        {
            movementComp = movementComponent;
        }
        else
        {
            Debug.LogWarning("Movemnt logic is missing required component UnitMovement");

            this.enabled = false;
        }
    }

    public override void MovementCmd(Vector3 Position)
    {
        base.MovementCmd(Position);

        movementComp.SetMovementTarget(Position);

        Unit.SetState(command);
    }

    public override void StopCommand()
    {
        base.StopCommand();

        movementComp.StopComponent();
    }

    public override void LogicUpdate()
    {
        base.LogicUpdate();

        if (movementComp.movementDone == true)
        {
            Unit.SetState(CmdLogic.commandType.StopCmd);
        }
    }
}
