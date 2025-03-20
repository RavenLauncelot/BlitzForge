using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Pathfinding;

public struct UnitMovementECS : IComponentData
{
    public float3 targetPosition;
    public bool movementDone;
    public int currentWaypoint;
    public float speed;
    public Path path;
}

