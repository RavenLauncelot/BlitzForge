using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class Unit : MonoBehaviour
{
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

    public enum command
    {
        move,
        attack
    }

    protected float health;
    protected float speed;

    protected bool canAttack;
    protected bool canMove;

    List<CommandTypes> commandList;
    CommandTypes activeCMD;

    public void SetCommand(CommandTypes command)
    {
        
    }   

    public class CommandTypes
    {
        command CMD;
    }

    public class CMDMove : CommandTypes
    {
        CMD = command.move;
    }

    public class CMDAttack : CommandTypes
    {

    }
}


