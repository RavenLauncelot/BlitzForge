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
        managerType = "MovementManager";
        unitIds = GetIds();

        foreach(int unit in unitIds)
        {
            int unitDataIndex = manager.unitDataIndexLookup[unit];
            MovementData movementData = manager.GetModuleData(unit, managerType) as MovementData;

            movementData.agent = manager.unitData[unitDataIndex].unitScript.GetComponent<NavMeshAgent>();
            movementData.agent.stoppingDistance = movementData.stoppingDistance;
            movementData.agent.acceleration = movementData.acceleration;
            movementData.agent.speed = movementData.maxSpeed;
        }
    }

    public void BasicMovementCommand(int[] instanceIds, Vector3 target)
    {
        for (int i = 0; i < instanceIds.Length; i++)
        {
            MovementData movementData = null;

            if (unitIds.Contains(instanceIds[i]))
            {
                movementData = manager.GetModuleData(instanceIds[i], managerType) as MovementData;

                movementData.agent.isStopped = false;
                movementData.reachedTarget = false;
                movementData.currentTarget = target;
                movementData.agent.SetDestination(target);
            }              
        }
    }

    public override void StopCommands(int[] ids)
    {
            MovementData movementData = null;

        for (int i = 0; i < ids.Length; i++)
        {
            if (unitIds.Contains(ids[i]))
            {
                movementData = manager.GetModuleData(ids[i], managerType) as MovementData;
            }

            movementData.reachedTarget = true;
            movementData.currentTarget = movementData.agent.transform.position;
            movementData.agent.isStopped = true;
        }
    }

    public void Update()
    {
        foreach(int id in unitIds)
        {
            if (id == 0)
            {
                continue;
            }

            MovementData movementData = manager.GetModuleData(id , managerType) as MovementData;

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

    public override void SetCommand(CommandData command)
    {
        if (command.commandType == "BasicMovementCommand")
        {
            BasicMovementCommand(command.selectedUnits, command.targettedArea[0]);
        }
    }
}

[System.Serializable]
public class MovementData : ModuleData
{
    public MovementData()
    {
        moduleType = "MovementManager";
    }

    public override ModuleData Clone()
    {
        return new MovementData
        {
            moduleType = moduleType,
            maxSpeed = maxSpeed,
            acceleration = acceleration,
            stoppingDistance = stoppingDistance,
        };
    }

    public float maxSpeed;
    public float acceleration;
    public float stoppingDistance;

    public Vector3 currentTarget;
    public bool reachedTarget;
    public NavMeshAgent agent;
}
