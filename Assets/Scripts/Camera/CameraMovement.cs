using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    //controls class
    PlayerControls cameraControls;

    //input actions
    InputAction orbitInput;
    InputAction movement;
    InputAction mouseInput;
    InputAction zoomInput;

    //variables for input
    Vector2 movementVec;
    Vector2 mouseVec;
    float cameraZoom;

    //when enabled camera will take mouse input and orbit the camera
    private bool orbitEnabled;

    //Transforms for moving the camera
    [SerializeField] Transform camTargetX;
    [SerializeField] Transform camTargetY;

    [SerializeField] Transform camPivotY;
    [SerializeField] Transform camPivotX;
    [SerializeField] Transform cameraTrans;

    //variables for speed ,sensitivty ,min max values and the scrtipableObj class
    public CameraData cameraData;

    //camera movement
    float movementSpeed;
    float movementSlerpSpeed;
    float terrainHeight;
    float cameraHeightOffset;

    //camera view
    float orbitSens;
    float camOrbitSlerpSpeed;

    float zoomSens;
    float zoomSlerpSpeed;
    float zoomMovement;
    float maxZoom;
    float minZoom;

    float minXAngle;
    float maxXAngle;

    float yMovement = 0;
    float currentX = 10;

    private void Awake()
    {
        cameraControls = new PlayerControls();
    }

    private void OnEnable()
    {
        //setting up input 
        orbitInput = cameraControls.BattleCamera.Orbit;
        movement = cameraControls.BattleCamera.CameraMovement;
        mouseInput = cameraControls.BattleCamera.MouseInput;
        zoomInput = cameraControls.BattleCamera.Zoom;

        orbitInput.started += enableOrbit;
        orbitInput.canceled += disableOrbit;

        orbitInput.Enable();
        movement.Enable();
        mouseInput.Enable();
        zoomInput.Enable();

        //getting data from scriptableObj
        movementSpeed = cameraData.movementSpeed;
        orbitSens = cameraData.orbitSens;
        zoomSens = cameraData.zoomSens;
        maxZoom = cameraData.maxZoom;
        minZoom = cameraData.minZoom;    
        camOrbitSlerpSpeed = cameraData.camOrbitSlerpSpeed;
        minXAngle = cameraData.minXAngle;
        maxXAngle = cameraData.maxXAngle;
        zoomSlerpSpeed = cameraData.zoomSlerpSpeed;
        maxZoom = cameraData.maxZoom;
        minZoom = cameraData.minZoom;

        currentX = camTargetX.eulerAngles.x;
        zoomMovement = cameraTrans.position.z;
    }

    private void OnDisable()
    {
        orbitInput.Disable();
        movement.Disable();
        mouseInput.Disable(); 
        zoomInput.Disable();
    }

    // Update is called once per frame
    void Update()
    {
        viewMovement();
        cameraMovement();
    }

    void viewMovement()
    {       
        //for the zoom 
        cameraZoom = zoomInput.ReadValue<float>();        
        cameraZoom *= zoomSens;    //because the scroll wheel is pressed like a button it doesn't need a time.delta and usually it has a value for one frame at a time
        
        zoomMovement += Mathf.Clamp(cameraZoom, -maxZoom - zoomMovement, -minZoom - zoomMovement);  //These are the wrong way round on purpase since it's in negative numbers of z

        //Slerping movement for zoom
        cameraTrans.localPosition = Vector3.Lerp(cameraTrans.localPosition, new Vector3(0,0,zoomMovement), Time.deltaTime * zoomSlerpSpeed);

        Debug.Log(Time.deltaTime);


        //camera orbit 
        if (orbitEnabled) //when enabled it will read value from the mouse and set the targets positionf or the camera
        {
            mouseVec = mouseInput.ReadValue<Vector2>();
            mouseVec *= (orbitSens * Time.deltaTime);   
            yMovement = mouseVec.x;
           
            mouseVec.y = Mathf.Clamp(mouseVec.y, minXAngle-currentX, maxXAngle-currentX);
            currentX += mouseVec.y;

            camTargetY.Rotate(0, yMovement, 0);
            camTargetX.Rotate(mouseVec.y,0,0);
        }

        //the slerp is placed here so that once it's finihsed being pressed it won't come to a sudden stop
        camPivotX.localRotation = Quaternion.Slerp(camPivotX.localRotation, camTargetX.localRotation, Time.deltaTime * camOrbitSlerpSpeed);
        camPivotY.localRotation = Quaternion.Slerp(camPivotY.localRotation, camTargetY.localRotation, Time.deltaTime * camOrbitSlerpSpeed);

        Debug.DrawRay(camPivotX.position, camPivotX.forward * -cameraTrans.position.z, Color.red);
        Debug.DrawRay(camTargetX.position, camTargetX.forward * -cameraTrans.position.z, Color.blue);
    }

    void cameraMovement()
    {
        movementVec = movement.ReadValue<Vector2>();
        movementVec *= (movementSpeed * Time.deltaTime);

        RaycastHit hit;
        if (Physics.Raycast(transform.position + new Vector3(0, 100, 0), Vector3.down, out hit))
        {
            terrainHeight = hit.point.y;
        }
        else
        {
            terrainHeight = 0;
        }

        terrainHeight += cameraHeightOffset;

        Vector3 SlerpTarget = camTargetY.TransformPoint(camTargetY.localPosition + new Vector3(movementVec.x, 0, movementVec.y));
        SlerpTarget += new Vector3(0, terrainHeight, 0);

        transform.position = Vector3.Slerp(transform.position, SlerpTarget, movementSlerpSpeed * Time.deltaTime);

        Debug.DrawRay(transform.position + new Vector3(0, 100, 0), Vector3.down * 200, Color.black);
    }

    void enableOrbit(InputAction.CallbackContext input)
    {
        orbitEnabled = true;
    }

    void disableOrbit(InputAction.CallbackContext input)
    {
        orbitEnabled = false;
    }
}
