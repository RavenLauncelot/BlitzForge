using Sirenix.OdinInspector.Editor.Modules;
using UnityEngine;
using UnityEngine.Rendering;

public class ModuleManager : MonoBehaviour
{
    protected UnitManager manager;
    protected LevelManager levelManager;

    private bool initialised = false;

    public void initModule(UnitManager unitManager, LevelManager LevelManager)
    {
        if (initialised)
        {
            return;
        }

        manager = unitManager;
        levelManager = LevelManager;
        initialised = true;        
    }

    public int[] GetIds(ModuleData.ModuleType type)
    {
        return manager.findUnitsWithModule(type);
    }
}

[CreateAssetMenu(fileName = "ModuleDataScriptable", menuName = "Scriptable Objects/ModuleData")]
public class ModuleDataScriptable : ScriptableObject
{

}
