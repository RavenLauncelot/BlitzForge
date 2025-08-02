using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CommandChanger : MonoBehaviour
{
    [SerializeField] private CommandData[] availableCommands;
    Dictionary<string, CommandData> commands;

    UnitHandler unitHandler;

    private void Start()
    {
        commands = new Dictionary<string, CommandData>();
        commands = availableCommands.ToDictionary(val => val.commandType, val => val);

        unitHandler = GameObject.Find("PlayerController(Clone)").GetComponentInChildren<UnitHandler>();    
    }

    public void ChangeCommand(string commandType)
    {
        unitHandler.ChangeCommand(commands.GetValueOrDefault(commandType));
    }
}
 