using UnityEngine;

[CreateAssetMenu(fileName = "AttackModuleData", menuName = "Scriptable Objects/AttackModuleData")]
public class AttackModuleData : ScriptableObject
{
    public float range;
    public float damage;
    public float reloadTime;
}
