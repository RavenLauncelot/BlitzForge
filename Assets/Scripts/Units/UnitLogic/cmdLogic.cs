using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using static UnitComponent;

public class CmdLogic : MonoBehaviour 
{
    public enum commandType
    {
        MovementCmd,
        AttackCmd,
        StopCmd
    }

    [SerializeField] protected commandType command;
    [SerializeField] protected Unit Unit;

    public virtual void MovementCmd(Vector3 position)
    {

    }

    public virtual void LogicUpdate()
    {

    }

    public commandType getCmdType()
    {
        return command;
    }

    public virtual void StopCommand()
    {

    }
}
