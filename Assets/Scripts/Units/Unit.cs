using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class Unit : MonoBehaviour
{
    UnitManager manager;

    public enum team
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

    protected float health;
    protected float speed;

    protected bool canAttack;
    protected bool canMove;

    List<CommandTypes> commandList;
    CommandTypes activeCMD;

    public void SetCommand(CommandTypes command)
    {
        activeCMD = command;
        commandList.Clear();
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

    private void commandTick(object sender, EventArgs e)  //this will check current commands. if there completed/incompleted, or new commands are issued.
    {
        //this might not be needed as this may not improve performance significantly enough
    }

    private void navTick(object sender, EventArgs e)   //this will update the nav to check for blockages and other thigns.
    {

    }
}


