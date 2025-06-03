using UnityEngine;

public class ManagerData
{
    public enum ModuleType
    {
        None,
        DetectionModule,
        AttackModule,
        MovementModule,
    }

    public ModuleType compType;
}
