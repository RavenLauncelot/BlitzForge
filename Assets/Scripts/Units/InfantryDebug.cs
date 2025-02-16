using UnityEngine;

public class InfantryDebug : Unit
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Unit.commandStates[] unitCommands = { Unit.commandStates.Move };

        this.initUnit(unitCommands);       
    }

    private void Update()
    {
        if (currentState == Unit.commandStates.Move)
        {
            Move();
        }
    }

    public override void MoveCommand(Vector3 Position)
    {
        UnitComponent unitComp = FindComponent(UnitComponent.UnitComponents.UnitMovement);
        
        unitComp.SetMovementTarget(Position);   

        currentState = Unit.commandStates.Move;
    }

    private void Move()
    {
        //some specefic stuff it does when moving 

        //movement is complete back to idle
        if (transform.GetComponent<UnitMovement>().movementDone == true)
        {
            currentState = commandStates.Idle;
        }
    }

    private void Attack()
    {
        //Some stuff it does when attacking 

        //runs after unit needs to udpate position 
        //specficially fire at enemy 
    }
}
