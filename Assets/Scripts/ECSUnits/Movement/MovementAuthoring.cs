using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Pathfinding;

public class MovementAuthoring : MonoBehaviour
{
    private class Baker : Baker<MovementAuthoring>
    {
        public override void Bake(MovementAuthoring author)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, new UnitMovementECS
            {
                targetPosition = float3.zero,
                path = AstarPath.,
                movementDone = true,
                currentWaypoint = 0,
                speed = 10
            });
        }
    }
}
