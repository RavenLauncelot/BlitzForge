using UnityEngine;

public enum command
{
    move,
    attack
}

public class CommandTypes
{
    public command CMD;
    bool done = false;
}

public class CMDMove : CommandTypes
{
    Vector3 position;

    CMDMove(Vector3 movePos)
    {
        CMD = command.move;
        position = movePos;
    }
}

public class CMDAttack : CommandTypes
{
    GameObject targetUnit;

    CMDAttack(GameObject unit)
    {
        targetUnit = unit;
        CMD = command.attack;
    }
}
