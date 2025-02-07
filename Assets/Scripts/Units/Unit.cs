using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using Pathfinding;

public class Unit : MonoBehaviour
{
    protected UnitManager manager;

    public enum teamName
    {
        teamA,
        teamB, 
        teamC,
        teamD,
        teamE,
        teamF,
        teamG,
        teamH,
    }

    public enum unitState
    {
        move,
        attack
    }

    protected float health;
    protected float speed;

    protected bool canAttack;
    protected bool canMove;

    protected teamName team;

    protected unitState state;

    protected List<CommandTypes> commandList;
    protected CommandTypes activeCMD;

    public void SetCommand(CommandTypes command)
    {
        activeCMD = command;
        commandList.Clear();

        initNewCommand();
    }    

    private void OnEnable()
    {
        manager.commandTick += commandTick;
        manager.navTick += navTick;
    }

    private void OnDisable()
    {
        manager.commandTick -= commandTick;
        manager.navTick -= navTick; 
    }

    private void initNewCommand()
    {
        switch (activeCMD.CMD)
        {
            case (CommandTypes.command.move):   //move
                Debug.Log("New command move - postion: " + activeCMD.movePos);
                break;
            case (CommandTypes.command.attack):   //attack
                Debug.Log("New command attack - target: " + activeCMD.targetObj.name);
                break;
            default:
                break;
        }
    }

    private void commandTick(object sender, EventArgs e)  //this will check current commands. if there completed/incompleted, or new commands are issued.
    {
        //this might not be needed as this may not improve performance significantly enough

        if (activeCMD.CMD == CommandTypes.command.move)  //move command
        {
            Debug.Log("Command tick: current cmd move");
        }

        else if (activeCMD.CMD == CommandTypes.command.attack)
        {
            Debug.Log("Command tick: current cmd attack");
        }
    }

    private void navTick(object sender, EventArgs e)   //this will update the nav to check for blockages and other thigns.
    {
        Debug.Log("Nav tick");
    }

    private void Start()
    {
        
    }
}


