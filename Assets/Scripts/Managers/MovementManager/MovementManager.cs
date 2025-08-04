using System.IO;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEditor.Analytics;
using UnityEngine;
using UnityEngine.AI;

public class MovementManager : ModuleManager
{

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

    public override void UnregisterModule(int id)
    {
        //I didn't cast them to movement modules when the script starts so yeah i guess
      
        managedModules = managedModules.Where(val => val.InstanceId != id).ToArray();
    }

    private void MovementCommand(int[] instanceIds, Vector3 target)
    {
        Vector3[] positions = NavmeshTools.EvenSpacing(instanceIds.Length, target, 10);

        for (int i = 0; i < positions.Length; i++)
        {
            MovementModule movementModule = null;

            if (moduleIdLookup.TryGetValue(instanceIds[i], out UnitModule unitModule))
            {
                movementModule = unitModule as MovementModule;

                movementModule.Agent.isStopped = false;
                movementModule.ReachedTarget = false;
                movementModule.CurrentTarget = target;
                movementModule.Agent.SetDestination(positions[i]);
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
        }

        else
        {
            Debug.Log("Command: " + command.commandType + " doesn't exist");
        }
    }
}
