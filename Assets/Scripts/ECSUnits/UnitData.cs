using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
public struct UnitData : IComponentData
{
    public float3 direction;
    public float speed;
}
