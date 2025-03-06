using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Entities.Serialization;
public partial struct UnitSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<UnitData>();
    }

    public void OnUpdate(ref SystemState state)
    {
        UnitJob job = new UnitJob
        {
            deltaTime = SystemAPI.Time.DeltaTime
        };

        job.ScheduleParallel();

        Debug.Log("On update");
    }

    public partial struct UnitJob : IJobEntity
    {
        public float deltaTime;

        public void Execute(ref UnitData unit, ref LocalTransform transform)
        {
            transform = transform.Translate(unit.direction * unit.speed * deltaTime);
        }
    }
}



