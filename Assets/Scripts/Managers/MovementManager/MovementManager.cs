using System.IO;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.AI;

public class MovementManager : ModuleManager
{
    public static MovementManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void Start()
    {
        foreach (UnitModule unit in managedModules)
        {
            MovementModule movementModule = unit as MovementModule;

            movementModule.Agent.stoppingDistance = movementModule.StoppingDistance;
            movementModule.Agent.acceleration = movementModule.Acceleration;
            movementModule.Agent.speed = movementModule.MaxSpeed;
        }
    }

    public void MovementCommand(int[] instanceIds, Vector3 target)
    {
        for (int i = 0; i < instanceIds.Length; i++)
        {
            MovementModule movementModule = null;

            if (moduleIdLookup.TryGetValue(instanceIds[i], out UnitModule unitModule))
            {
                movementModule = unitModule as MovementModule;

                movementModule.Agent.isStopped = false;
                movementModule.ReachedTarget = false;
                movementModule.CurrentTarget = target;
                movementModule.Agent.SetDestination(target);

                Debug.Log("Movement command set: " + target);
            }                
        }
    }

    public override void StopCommands(int[] instanceIds)
    {
        MovementModule movementModule = null;

        for (int i = 0; i < instanceIds.Length; i++)
        {
            if (moduleIdLookup.TryGetValue(instanceIds[i], out UnitModule unitModule))
            {
                movementModule = unitModule as MovementModule;

                movementModule.ReachedTarget = true;
                movementModule.CurrentTarget = movementModule.Agent.transform.position;
                movementModule.Agent.isStopped = true;
            }
        }
    }

    public void Update()
    {
        foreach(UnitModule module in managedModules)
        {
            MovementModule movementModule = module as MovementModule;

            if (movementModule.ReachedTarget == true)
            {
                movementModule.ReachedTarget = true;
                movementModule.Agent.isStopped = true;
            }

            else if(movementModule.Agent.remainingDistance < movementModule.StoppingDistance)
            {     
                movementModule.ReachedTarget = true;
                movementModule.Agent.isStopped = true;           
            }
        }
    }

    public override void SetCommand(CommandData command)
    {
        if (command.commandType == "MovementCommand")
        {
            MovementCommand(command.selectedUnits, command.targettedArea[0]);

            Debug.Log("Command sent");
        }

        else
        {
            Debug.Log("Command: " + command.commandType + " doesn't exist");
        }
    }
}
