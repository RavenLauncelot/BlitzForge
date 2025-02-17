using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem;

public class UnitDictator : MonoBehaviour
{
    List<Unit> selectedUnits;
    UnitSelector unitSelector;

    public commandStates currentlySetCommand;

    private PlayerControls playerControls;
    private InputAction startAction;
    private InputAction mousePos;

    Camera cam;

    private void OnEnable()
    {
        playerControls = new PlayerControls();
        startAction = playerControls.UnitControls.StartAction;\
        mousePos = playerControls.UnitControls.MousePos;

        startAction.performed += setCommand;

        cam = Camera.main;
    }

    public enum commandStates
    {
        Stop,
        Move,
        Attack
    }

    private void setCommand(InputAction.CallbackContext input)
    {
        switch (currentlySetCommand)
        {
            case commandStates.Stop:
                break;
            case commandStates.Move:
                SetMovementCmd();
                break;
            case commandStates.Attack:
                break;
            default:
                break;
        }
    }

    private void SetMovementCmd()
    {
        Vector2 mousePosition = mousePos.ReadValue<Vector2>();
        Vector3 screenPos = new(mousePosition.x, mousePosition.y, cam.nearClipPlane);

        if (Physics.Raycast(cam.ScreenPointToRay(screenPos, Camera.MonoOrStereoscopicEye.Mono), out RaycastHit screenRay))
        {
            //debug int
            int counter = 0;

            //Command cmd = new Command();
            //cmd.MoveCommand(screenRay.point);

            foreach (Unit unit in selectedUnits)
            {
                if (unit.CheckCommand(UnitDictator.commandStates.Move))
                {
                    unit.MoveCommand(screenRay.point);
                }

                counter++;
            }

            Debug.Log("Command given to " + counter + "units");
        }
    }

    private void SetStopCmd()
    {

    }

    private void SetAttackCmd()
    {

    }

    public void GetSelectedUnits()
    {
        selectedUnits = unitSelector.getSelectedUnits();
    }
}
