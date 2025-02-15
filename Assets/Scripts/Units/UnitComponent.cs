using System.Data;
using System.Xml.Serialization;
using UnityEngine;

public class UnitComponent : MonoBehaviour
{
    public Unit unit;
    public CommandTypes commandType;

    public virtual void CommandTick()
    {

    }

    public virtual void NavTick()
    {

    }
}
