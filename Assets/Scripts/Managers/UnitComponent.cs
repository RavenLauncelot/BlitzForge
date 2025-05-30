using UnityEngine;

public class UnitComponent
{
    public enum ComponentType
    {
        None,
        DetectionComp,
        AttackComp,
        MovementComp,
    }

    public ComponentType compType;
}
