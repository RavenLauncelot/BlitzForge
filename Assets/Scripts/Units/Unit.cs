using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class Unit : MonoBehaviour
{
    //Unit behaviours
    protected UnitComponent[] unitComponents;

    //Unit stats
    protected float health;
    protected float speed;

    //Team name
    //public UnitCommander.teamName teamName;
    
    //Command methods
    

    //Available commands
    UnitDictator.commandStates[] commands;

    //holds current state of unit
    [SerializeField] protected UnitDictator.commandStates currentState = UnitDictator.commandStates.Idle;

    public void initUnit(UnitDictator.commandStates[] setCommands)
    {
        commands = setCommands;

        //find attached components
        unitComponents = GetComponents<UnitComponent>();
    }

    private void NavigationTick(object sender, EventArgs e)
    {
        foreach(UnitComponent component in unitComponents)
        {
            component.NavTick();
        }
    }
    private void CommandTick(object sender, EventArgs e)
    {
        foreach (UnitComponent component in unitComponents)
        {
            component.CommandTick();
        }
    }

    //checks if command is valid
    public bool CheckCommand(UnitDictator.commandStates checkCommand)
    {
        return commands.Contains(checkCommand);
    }

    protected UnitComponent FindComponent(UnitComponent.UnitComponents componentEnum)
    {
        foreach (UnitComponent unitComp in unitComponents)
        {
            if (unitComp.componentType == componentEnum)
            {
                return unitComp;
            }
        }

        return null;
    }
   

    //Command setters
    //These will set a command and then change the state of the unit
    //These will be defined in the specific units logic
    public virtual void AttackCommand()
    {

    }

    public virtual void MoveCommand(Vector3 position)
    {
        
    }

    public virtual void IdleCommand()
    {
        
    }
}


