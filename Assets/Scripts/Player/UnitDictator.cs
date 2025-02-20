using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;


public class UnitDictator : MonoBehaviour
{
    [SerializeField] List<Unit> selectedUnits;
    public UnitSelector unitSelector;

    public commandStates currentlySetCommand;

    private PlayerControls playerControls;
    private InputAction startAction;
    private InputAction mousePos;

    Camera cam;

    //ignore for now
    public float positionMod;

    private void OnEnable()
    {
        playerControls = new PlayerControls();
        startAction = playerControls.UnitControls.StartAction;
        mousePos = playerControls.UnitControls.MousePos;

        startAction.performed += SetCommand;

        mousePos.Enable();
        startAction.Enable();

        cam = Camera.main;
    }

    private void OnDisable()
    {
        startAction.performed -= SetCommand;

        startAction.Disable();
        mousePos.Disable();
    }

    public enum commandStates
    {
        Idle,
        Move,
        Attack
    }

    private void SetCommand(InputAction.CallbackContext input)
    {
        Debug.Log("Bruh");

        selectedUnits = unitSelector.GetSelectedUnits();

        switch (currentlySetCommand)
        {
            case commandStates.Idle:
                SetStopCmd();
                break;               
            case commandStates.Move:
                SetMovementCmd();
                break;
            case commandStates.Attack:
                SetAttackCmd();
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
            int unitCount = selectedUnits.Count;
            List<Vector3> unitPositions = new List<Vector3>();

            //checks if square number 
            if (Mathf.Sqrt(unitCount) % 1  == 0)
            {
                Debug.Log("Square Number");

                float result = Mathf.Sqrt(unitCount);
                for (int x = 0; x < result; x++)
                {
                    for (int y = 0; y < result; y++)
                    {
                        unitPositions.Add(new Vector3(x, 0, y));
                    }
                }
            }

            //if its not a square number
            else
            {
                Debug.Log("Not Sqaure");            

                float result = Mathf.Sqrt(unitCount);
                result -= result % 1;
                float remainder = unitCount - (result * result);

                Debug.Log("Not Square result: " + result);

                for (int x = 0; x < result; x++)
                {
                    for (int y = 0; y < result; y++)
                    {
                        unitPositions.Add(new Vector3(x, 0, y));

                        Debug.Log(x + " and " + y + "Square Loop");
                    }
                }

                float xRemainder = 0;  
                float yRemainder = result;   //result is the square number so represents the height of the square. this is where the new line of units will start
                for (int i = 0; i < remainder; i++)
                {
                    if (xRemainder >= result)
                    {
                        xRemainder = 0;
                        yRemainder++;
                    }

                    Debug.Log(xRemainder + " and " + yRemainder + "Remainder Loop");

                    unitPositions.Add(new Vector3(xRemainder,0 , yRemainder));

                    xRemainder++;
                }
            }

            //applying modifier to positions
            for(int i = 0; i < unitPositions.Count; i++)
            {
                unitPositions[i] = unitPositions[i] * positionMod;
                Debug.Log(unitPositions[i]);
            }

            //applying command to each unit with position
            int unitCounter = 0;
            foreach (Unit unit in selectedUnits)
            {
                if (unit.CheckCommand(UnitDictator.commandStates.Move))
                {
                    unit.MoveCommand(screenRay.point + unitPositions[unitCounter]);
                }

                unitCounter++;
            }
        }

        else
        {
            //ray didnt return anything 
            return;
        }
    }

    private void SetStopCmd()
    {
        foreach (Unit unit in selectedUnits)
        {
            unit.IdleCommand();
        }
    }

    private void SetAttackCmd()
    {

    }
}
