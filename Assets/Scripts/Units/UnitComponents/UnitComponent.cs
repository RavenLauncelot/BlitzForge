using System.Data;
using System.Xml.Serialization;
using UnityEngine;

public class UnitComponent : MonoBehaviour
{
    public enum UnitComponents
    {
        UnitAttack,
        UnitMovement
    }

    public UnitComponents componentType;

    //stops any component doing manually assigned commands
    public virtual void StopComponent()
    {

    }

    public virtual void CommandTick()
    {

    }

    public virtual void NavTick()
    {

    }

    public virtual void SetMovementTarget(Vector3 position)
    {

    }
}
