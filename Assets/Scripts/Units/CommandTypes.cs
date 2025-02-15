using UnityEngine;

public enum CommandTypes
{
    Move,
    Attack
}

public class Command
{
    public CommandTypes commandType;
    public Vector3 positionData;
    public Unit unitData;
}

public class MoveCommand : Command
{
    public MoveCommand(Vector3 position)
    {
        commandType = CommandTypes.Move;
        positionData = position;
    }
}

public class AttackCommand : Command
{
    public AttackCommand(Unit targetUnit)
    {
        commandType = CommandTypes.Attack;
        unitData = targetUnit;
    }
}