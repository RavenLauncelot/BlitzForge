using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class Unit : MonoBehaviour
{
    //Unit behaviours
    protected CmdLogic[] unitLogic;

    //Team name
    //public UnitCommander.teamName teamName;

    //Available commands
    CmdLogic.commandType[] commands;

    //holds current state of unit
    [SerializeField] protected CmdLogic.commandType cmdState = CmdLogic.commandType.StopCmd;

    public void initUnit(CmdLogic.commandType[] setCommands)
    {
        commands = setCommands;

        //find attached components
        unitLogic = GetComponents<CmdLogic>();
    }

    private void Update()
    {
        foreach(CmdLogic cmdLogic in unitLogic)
        {
            if (cmdLogic.getCmdType() == cmdState)
            {
                cmdLogic.LogicUpdate();

                return;
            }
        }
    }

    //checks if command is valid
    public bool CheckCommand(CmdLogic.commandType checkCommand)
    {
        return commands.Contains(checkCommand);
    } 

    public void SetState(CmdLogic.commandType state)
    {
        this.cmdState = state;
    }

    public void StopAllCommands()
    {
        foreach (CmdLogic cmdLogic in unitLogic)
        {
            cmdLogic.StopCommand();
        }
    }
}


