using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.Rendering;
using System.Linq;
using Sirenix.Utilities;
using Unity.VisualScripting;

public class UnitHandler : MonoBehaviour
{
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private UnitManager manager;
    [SerializeField] private UnitManager.TeamId controlledTeam;

    PlayerControls UnitControls;

    InputAction leftButton;
    InputAction rightButton;
    InputAction mousePos;

    BoxCollider unitTrigger;
    Camera cam;
    [SerializeField] Transform camYaw;

    public List<Unit> selectedUnits;
    public List<Unit> targetedUnits;
    Vector3[] targettedBounds = new Vector3[4];

    private bool isTargeting;
    public bool IsTargeting { get { return isTargeting; } }
    
    private bool isSelecting;
    public bool IsSelecting { get { return isSelecting; } }

    Vector3 initialPoint;
    Vector3 currentPoint;

    [SerializeField] private List<CommandData> commandList;
    [SerializeField] private CommandData currentlySetCommand;

    private void OnEnable()
    {
        UnitControls = new PlayerControls();

        leftButton = UnitControls.UnitControls.Selection;
        rightButton = UnitControls.UnitControls.StartAction;
        mousePos = UnitControls.UnitControls.MousePos;

        leftButton.started += StartSelection;
        leftButton.canceled += EndSelection;

        rightButton.started += StartTargeting;
        rightButton.canceled += EndTargeting;

        leftButton.Enable();
        rightButton.Enable();
        mousePos.Enable();


        unitTrigger = GetComponent<BoxCollider>();
        cam = Camera.main;
    }

    private void OnDisable()
    {
        leftButton.Disable();
        rightButton.Disable();
        mousePos.Disable();
    }

    //Selecting Units
    private void StartSelection(InputAction.CallbackContext context)
    {
        ResetSelection();

        Vector2 mousePosition = mousePos.ReadValue<Vector2>();
        Vector3 screenPos = new(mousePosition.x, mousePosition.y, cam.nearClipPlane);

        if (Physics.Raycast(cam.ScreenPointToRay(screenPos, Camera.MonoOrStereoscopicEye.Mono), out RaycastHit screenRay, Mathf.Infinity, groundLayer))
        {
            isSelecting = true;
            initialPoint = screenRay.point;

            unitTrigger.enabled = true;
        }
        else
        {
            //did not hit map cancelled
            isSelecting = false;
        }
    }

    private void EndSelection(InputAction.CallbackContext context)
    {
        isSelecting = false;

        unitTrigger.enabled = false;
    }

    //Commanding Units
    private void StartTargeting(InputAction.CallbackContext context)
    {
        if (selectedUnits == null || selectedUnits.Count == 0)
        {
            return;
        }

        Vector2 mousePosition = mousePos.ReadValue<Vector2>();
        Vector3 screenPos = new(mousePosition.x, mousePosition.y, cam.nearClipPlane);

        if (Physics.Raycast(cam.ScreenPointToRay(screenPos, Camera.MonoOrStereoscopicEye.Mono), out RaycastHit screenRay, Mathf.Infinity, groundLayer))
        {
            isTargeting = true;
            initialPoint = screenRay.point;
            targettedBounds[0] = screenRay.point;

            unitTrigger.enabled = true;
        }
        else
        {
            //did not hit map cancelled
            isTargeting = false;
        }
    }

    private void EndTargeting(InputAction.CallbackContext context)
    {
        isTargeting = false;

        Vector2 mousePosition = mousePos.ReadValue<Vector2>();
        Vector3 screenPos = new(mousePosition.x, mousePosition.y, cam.nearClipPlane);

        targettedBounds = GetLowerBounds();

        //It should now send the command to the UnitManager
        if (selectedUnits != null)
        {
            CommandData newCommand = new CommandData
            {
                targetModule = currentlySetCommand.targetModule,
                commandType = currentlySetCommand.commandType,

                selectedUnits = GetIds(selectedUnits),
                targettedUnits = GetIds(targetedUnits),

                targettedArea = targettedBounds
            };

            manager.SendCommand(newCommand);
        }
    }

    private int[] GetIds(List<Unit> unitsToId)
    {
        int[] idsToReturn = new int[unitsToId.Count];

        if (unitsToId == null)
        {
            return null;
        }

         idsToReturn = unitsToId.Select(units => units.InstanceId).ToArray();
         return idsToReturn;
    }

    private void ResetSelection()
    {
        if (selectedUnits != null)
        {
            foreach (var unit in selectedUnits)
            {
                unit.UnitSelected(false);
            }
        }

        selectedUnits.Clear();

        if (targetedUnits != null)
        {
            foreach (var unit in targetedUnits)
            {
                unit.UnitSelected(false);
            }
        }

        targetedUnits.Clear();
    }

    private void Update()
    {
        if (isSelecting || isTargeting)
        {
            Vector2 mousePosition = mousePos.ReadValue<Vector2>();
            Vector3 screenPos = new(mousePosition.x, mousePosition.y, cam.nearClipPlane);

            if (Physics.Raycast(cam.ScreenPointToRay(screenPos, Camera.MonoOrStereoscopicEye.Mono), out RaycastHit screenRay))
            {
                Vector3 pointAtoB = screenRay.point - initialPoint;
                Vector3 pointAtoBLocal = transform.InverseTransformPoint(screenRay.point) - transform.InverseTransformPoint(initialPoint);

                pointAtoB = new(pointAtoB.x, 200, pointAtoB.z);
                pointAtoBLocal = new(pointAtoBLocal.x, 200, pointAtoBLocal.z);

                Vector3 centre = initialPoint + pointAtoB / 2;
                centre = new(centre.x, 0, centre.z);

                //selectionTrigger.transform.position = centre;
                //selectionTrigger.transform.localScale = size;

                unitTrigger.size = pointAtoBLocal;
                transform.position = centre;

                currentPoint = screenRay.point;
            }

            else
            {
                //lol it stays the same cus like the mouse didnt hit stuff
            }

            transform.rotation = camYaw.rotation;

            //Debug ray
            Vector2 debugMouse = mousePos.ReadValue<Vector2>();
            Vector3 yeahman = new(debugMouse.x, debugMouse.y, cam.nearClipPlane);
            Ray debugray = cam.ScreenPointToRay(yeahman, Camera.MonoOrStereoscopicEye.Mono);

            Debug.DrawRay(debugray.origin, debugray.direction * 20000, Color.yellow);
        }
    }

    /// <summary>
    /// Will return lower bounds of boxCollider order will differ but always connecting points indexes 1 and 3 are aproximated.
    /// </summary>
    public Vector3[] GetLowerBounds()
    {
        //Not going to use the collider for it we're gonna use the screen ray position and make a square in local space then converting it to world for the linerenderer.
        //We need to use local space as the box collider is alligned with it and finding a square in world space would only give the AABB (axis alligned bounding box) new word so cool
        Vector3 initialPointLocal = transform.InverseTransformPoint(initialPoint);

        //This just finds it's postion in relation to the initialPoint which can then be added on later to find the localPos
        Vector3 currentPointOffset = transform.InverseTransformPoint(currentPoint) - initialPointLocal;

        //find approximated y between the inital and current point
        float yValue = initialPointLocal.y + (currentPointOffset.y / 2);

        //Sometimes the postion of values can differ and hence the order 

        //FL FR BR BL
        //initial   C
        //D         current

        //BR BL FR FL
        //otherwise will be 
        //D         current
        //inital    C

        Vector3[] selectedArea = new Vector3[4]
        {
            initialPointLocal,
            new Vector3(initialPointLocal.x + currentPointOffset.x, yValue, initialPointLocal.z),
            initialPointLocal + currentPointOffset,
            new Vector3(initialPointLocal.x, yValue, initialPointLocal.z + currentPointOffset.z)
        };

        //converting the positions back to world space
        for (int i = 0; i < 4; i++)
        {
            selectedArea[i] = transform.TransformPoint(selectedArea[i]);
        }

        return selectedArea;
    }

    //I hate this the bit the most
    private void OnTriggerEnter(Collider other)
    {
        if (isSelecting)
        {
            if (other.gameObject.transform.root.TryGetComponent<Unit>(out Unit UnitScript))
            {
                if (UnitScript.TeamId == controlledTeam)
                {
                    if (!selectedUnits.Contains(UnitScript))
                    {
                        selectedUnits.Add(UnitScript);
                        UnitScript.UnitSelected(true);
                    }
                }
            }
        }

        else if (isTargeting)
        {
            if (other.gameObject.transform.root.TryGetComponent<Unit>(out Unit UnitScript))
            {
                if (UnitScript.TeamId != controlledTeam)
                {
                    if (!targetedUnits.Contains(UnitScript))
                    {
                        targetedUnits.Add(UnitScript);
                        UnitScript.UnitSelected(true);
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (isSelecting)
        {
            if (other.gameObject.transform.root.TryGetComponent<Unit>(out Unit UnitScript))
            {
                if (UnitScript.TeamId == controlledTeam)
                {
                    //Debug.Log("Outside selected area");
                    selectedUnits.Remove(UnitScript);
                    UnitScript.UnitSelected(false);
                }
            }
        }

        else if (isTargeting)
        {
            if (other.gameObject.transform.root.TryGetComponent<Unit>(out Unit UnitScript))
            {
                if (UnitScript.TeamId != controlledTeam)
                {
                    //Debug.Log("Outside selected area");
                    targetedUnits.Remove(UnitScript);
                    UnitScript.UnitSelected(false);
                }
            }
        }
    }
}
