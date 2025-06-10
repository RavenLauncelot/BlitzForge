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

    public ModuleType compType;

    public virtual void SetValues(ModuleDataScriptable data)
    {

    }
}
