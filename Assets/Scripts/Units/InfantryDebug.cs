using System.Linq;
using UnityEngine;

public class InfantryDebug : Unit
{
    //this will be set in the inspector
    [SerializeReference] public UnitDictator.commandStates[] unitCommands;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.initUnit(unitCommands);       
    }

    private void Update()
    {
        switch (currentState)
        {
            case(UnitDictator.commandStates.Idle):
                Idle();
                break;
            case(UnitDictator.commandStates.Move):
                Move();
                break;
            case(UnitDictator.commandStates.Attack):
                Attack();
                break;
            default:
                break;
        }
    }

    public override void MoveCommand(Vector3 Position)
    {
        UnitComponent unitComp = FindComponent(UnitComponent.UnitComponents.UnitMovement);
        
        unitComp.SetMovementTarget(Position);   

        currentState = UnitDictator.commandStates.Move;
    }

    private void Move()
    {
        //some specefic stuff it does when moving 

        //movement is complete back to idle
        if (transform.GetComponent<UnitMovement>().movementDone == true)
        {
            currentState = UnitDictator.commandStates.Idle;
        }
    }

    private void Attack()
    {
        //Some stuff it does when attacking 

        //runs after unit needs to udpate position 
        //specficially fire at enemy 
    }

    private void Idle()
    {

    }
}
