using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
public struct UnitData : IComponentData
{
    public enum commandType
    {
        move,
        attack,
        idle
    }

    public float health;
    public commandType currentCommand;
}
