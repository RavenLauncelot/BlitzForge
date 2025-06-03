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

    public int[] GetIds(ManagerData.ModuleType type)
    {
        return manager.findUnitsWithModule(type);
    }
}
