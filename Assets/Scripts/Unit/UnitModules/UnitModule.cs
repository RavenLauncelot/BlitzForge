using UnityEngine;

public class UnitModule : MonoBehaviour
{
    public virtual ManagerData GetManagerData()
    {
        Debug.LogWarning("Overide not defined in unitModule, return null manager data");

        return new ManagerData();
    }
}
