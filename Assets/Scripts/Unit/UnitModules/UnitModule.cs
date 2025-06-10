using UnityEngine;

public class UnitModule : MonoBehaviour
{
    public virtual ModuleData GetManagerData()
    {
        Debug.LogWarning("Overide not defined in unitModule, return null manager data");

        return new ModuleData();
    }
}
