using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using JetBrains.Annotations;
public class UnitAuthoring : MonoBehaviour
{
    public float speed;
    public float3 direction;

    private class Baker : Baker<UnitAuthoring>
    {
        public override void Bake(UnitAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new UnitData
            {
               health = 0
            });
        }
    }
}
