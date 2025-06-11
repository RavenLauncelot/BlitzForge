using Sirenix.OdinInspector.Editor.Modules;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class ModuleManager : MonoBehaviour
{
    protected UnitManager manager;
    protected LevelManager levelManager;

    private bool initialised = false;

    [SerializeReference] protected int[] unitIds;

    protected ModuleKind moduleType;
    public ModuleKind ModuleType {  get { return moduleType; } }
    public enum ModuleKind
    {
        None,
        AttackModule,
        DetectionModule,
        MovementModule,
    }

    public void InitModule(UnitManager unitManager, LevelManager LevelManager)
    {
        if (initialised)
        {
            return;
        }

        manager = unitManager;
        levelManager = LevelManager;
        initialised = true;        
    }

    public int[] GetIds(ModuleManager.ModuleKind type)
    {
        return manager.GetIdsWithModule(type);
    }

    public virtual void StopCommands(int instanceId)
    {

    }
}


public class ModuleData
{
    public ModuleManager.ModuleKind moduleType;
}


public class ModuleDataConstructor : ScriptableObject
{
    public virtual ModuleData GetNewData()
    {
        return new ModuleData();
    }
}
