using NUnit.Framework;
using UnityEngine;
using Pathfinding;
using System.Collections.Generic;
using Unity.Android.Types;
using System;

public class Unit : MonoBehaviour
{
    //Unit behaviours
    UnitComponent[] unitComponents;
    [SerializeField] List<CommandTypes> availableCommands;

    //Unit stats
    [SerializeField] private float health;
    [SerializeField] private float speed;

    //active commands list
    [SerializeField] private List<Command> commandList;

    //Team name
    public UnitCommander.teamName teamName;

    //new command 
    public Command addedCommand;

    private void Start()
    {
        //find attached components/behaviours
        unitComponents = transform.GetComponents<UnitComponent>();
        foreach(UnitComponent unitCom in unitComponents)
        {
            availableCommands.Add(unitCom.commandType);
            unitCom.unit = this;
        }
    }

    public void AddCommand()
    {
        if (checkCommand(addedCommand))
        {
            ClearCommands();
            commandList.Add(addedCommand);
        }
    }

    public void QueueCommand(Command command)
    {
        if (checkCommand(command))
        {
            commandList.Add(command);
        }
    }

    public void ClearCommands()
    {
        commandList.Clear();
    }

    public Command GetCommand()
    {
        return commandList[0];
    }

    public bool checkCommand(Command command)
    {
        return commandList.Contains(command);
    }

    public void CommandCompleted()
    {
        commandList.RemoveAt(0);
    }

    public void NavigationTick(object sender, EventArgs e)
    {
        foreach(UnitComponent component in unitComponents)
        {
            component.NavTick();
        }
    }

    public void CommandTick(object sender, EventArgs e)
    {
        foreach (UnitComponent component in unitComponents)
        {
            component.CommandTick();
        }
    }
}


