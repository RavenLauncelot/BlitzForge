using UnityEngine;

public class CommandTypes
{
    public enum command
    {
        move,
        attack
    }

    public command CMD;
    public bool done = false;

    public Vector3 movePos;
    public GameObject targetObj;

    public CommandTypes(command setCMD, Vector3 position)
    {
        movePos = position;
        CMD = setCMD;
    }

    public CommandTypes(command setCMD, GameObject target)
    {
        targetObj = target;
        CMD = setCMD;
    }
}


