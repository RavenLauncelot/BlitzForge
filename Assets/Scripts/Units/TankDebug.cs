using System.Linq;
using UnityEngine;

public class TankDebug : Unit
{
    //this will be set in the inspector
    [SerializeReference] public CmdLogic.commandType[] unitCommands;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.initUnit(unitCommands);       
    }
}
