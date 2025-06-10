using UnityEngine;

public class ModuleData
{
    public enum ModuleType
    {
        None,
        DetectionModule,
        AttackModule,
        MovementModule,
    }

    public ModuleType moduleType;
}
