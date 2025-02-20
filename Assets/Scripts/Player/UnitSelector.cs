using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEditor.SceneManagement;

public class UnitSelector : MonoBehaviour
{
    [SerializeField] public List<Unit> selectedUnits;

    PlayerControls UnitControls;

    InputAction selection;
    InputAction startAction;
    InputAction mousePos;

    BoxCollider selectionTrigger;
    Camera cam;

    Vector3 firstSelection;

    bool isSelecting = false;


    private void OnEnable()
    {
        UnitControls = new PlayerControls();

        selection = UnitControls.UnitControls.Selection;
        mousePos = UnitControls.UnitControls.MousePos;

        selection.started += StartSelection;
        selection.canceled += EndSelection;

        selection.Enable();
        mousePos.Enable();


        selectionTrigger = GetComponentInChildren<BoxCollider>();   
        cam = Camera.main;
    }

    private void OnDisable()
    {
        selection.Disable();
        mousePos.Disable();
    }

    private void StartSelection(InputAction.CallbackContext context)  //player press the leftbutton 
    {
        selectedUnits.Clear();

        Vector2 mousePosition = mousePos.ReadValue<Vector2>();
        Vector3 screenPos = new (mousePosition.x, mousePosition.y, cam.nearClipPlane);


        if (Physics.Raycast(cam.ScreenPointToRay(screenPos, Camera.MonoOrStereoscopicEye.Mono), out RaycastHit screenRay))
        {
            isSelecting = true;
            firstSelection = screenRay.point;

            selectionTrigger.enabled = true;
        }
        else
        {
            //did not hit map cancelled
            isSelecting = false;
        }
    }

    private void EndSelection(InputAction.CallbackContext context)  //player release the left button
    {
        isSelecting = false;

        selectionTrigger.enabled = false;
    }

    private void Update()
    {
        //if true it will make a large trigger in the area in which the mouse is. 
        //this will select units
        if (isSelecting)
        {
            Vector2 mousePosition = mousePos.ReadValue<Vector2>();
            Vector3 screenPos = new(mousePosition.x, mousePosition.y, cam.nearClipPlane);

            if (Physics.Raycast(cam.ScreenPointToRay(screenPos, Camera.MonoOrStereoscopicEye.Mono), out RaycastHit screenRay))
            {
                Vector3 selectionToPoint = screenRay.point - firstSelection;
                selectionToPoint = new(selectionToPoint.x,200,selectionToPoint.z);

                Vector3 centre = firstSelection + selectionToPoint / 2;
                centre = new(centre.x, 100, centre.z);

                //selectionTrigger.transform.position = centre;
                //selectionTrigger.transform.localScale = size;

                selectionTrigger.size = selectionToPoint;  
                selectionTrigger.center = centre;
            }

            else
            {
                //lol it stays the same cus like the mouse didnt hit stuff
            }
        }

        //Debug ray
        Vector2 debugMouse = mousePos.ReadValue<Vector2>();
        Vector3 yeahman = new(debugMouse.x, debugMouse.y, cam.nearClipPlane);
        Ray debugray = cam.ScreenPointToRay(yeahman, Camera.MonoOrStereoscopicEye.Mono);

        Debug.DrawRay(debugray.origin, debugray.direction * 20000, Color.yellow);
    }

    public List<Unit> GetSelectedUnits()
    {
        return selectedUnits;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.TryGetComponent<Unit>(out Unit UnitScript))
        {
            Debug.Log("Inside selected area");
            selectedUnits.Add(UnitScript);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.TryGetComponent<Unit>(out Unit UnitScript))
        {
            Debug.Log("Outside selected area");
            selectedUnits.Remove(UnitScript);
        }
    }
}
