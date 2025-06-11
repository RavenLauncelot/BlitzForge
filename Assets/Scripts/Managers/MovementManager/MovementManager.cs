using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.AI;

public class MovementManager : ModuleManager
{

    public void Start()
    {
        moduleType = ModuleKind.MovementModule;
        unitIds = GetIds(ModuleManager.ModuleKind.MovementModule);

        foreach(int unit in unitIds)
        {
            int unitDataIndex = manager.unitDataIndexLookup[unit];
            MovementData movementData = manager.GetModuleData(unit, ModuleManager.ModuleKind.MovementModule) as MovementData;

            movementData.agent = manager.unitData[unitDataIndex].unitScript.GetComponent<NavMeshAgent>();
            movementData.agent.stoppingDistance = movementData.stoppingDistance;
            movementData.agent.acceleration = movementData.acceleration;
            movementData.agent.speed = movementData.maxSpeed;
        }
    }

    public void SetMovementCommand(int instanceId, Vector3 target)
    {
        MovementData movementData = null;

        if (unitIds.Contains(instanceId))
        {
            movementData = manager.GetModuleData(instanceId, ModuleManager.ModuleKind.MovementModule) as MovementData;
        }

        movementData.agent.isStopped = false;
        movementData.reachedTarget = false;
        movementData.currentTarget = target;
        movementData.agent.SetDestination(target);
    }

    public override void StopCommands(int instanceId)
    {
        MovementData movementData = null;

        if (unitIds.Contains(instanceId))
        {
            movementData = manager.GetModuleData(instanceId, ModuleManager.ModuleKind.MovementModule) as MovementData;
        }

        movementData.reachedTarget = true;
        movementData.currentTarget = movementData.agent.transform.position;
        movementData.agent.path = null;
    }

    public void Update()
    {
        foreach(int id in unitIds)
        {
            MovementData movementData = manager.GetModuleData(id , ModuleManager.ModuleKind.MovementModule) as MovementData;

            if (movementData.reachedTarget == true)
            {

            }

            else if(movementData.agent.remainingDistance < movementData.stoppingDistance)
            {     
                movementData.reachedTarget = true;
                movementData.agent.isStopped = true;           
            }
        }
    }
}

public class MovementData : ModuleData
{
    public MovementData()
    {
        moduleType = ModuleManager.ModuleKind.MovementModule;
    }

    public float maxSpeed;
    public float acceleration;
    public float stoppingDistance;

    public Vector3 currentTarget;
    public bool reachedTarget;
    public NavMeshAgent agent;
}

[CreateAssetMenu(fileName = "MovementModuleData", menuName = "Scriptable Objects/ModuleData/MovementModuleData")]
public class MovementDataConstructor : ModuleDataConstructor
{
    public override ModuleData GetNewData()
    { 
        return new MovementData()
        {
            maxSpeed = maxSpeed,
            acceleration = acceleration,
            stoppingDistance = stoppingDistance,
        };
    }

    public float maxSpeed;
    public float acceleration;
    public float stoppingDistance;
}
