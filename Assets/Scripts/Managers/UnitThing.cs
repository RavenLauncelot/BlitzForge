using System.Data;
using System.Xml.Serialization;
using UnityEngine;

public class UnitThing : MonoBehaviour
{
    public enum UnitComponents
    {
        UnitTargeting,
        UnitMovement
    }

    public UnitComponents componentType;

    //stops any component doing manually assigned commands
    public virtual void StopComponent()
    {

    }
}
