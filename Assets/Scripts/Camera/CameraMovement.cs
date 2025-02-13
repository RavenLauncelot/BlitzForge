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
    float cameraTerrainOffset;

    float zoomSpeedMod;
    float shiftSpeedMod;

    Vector3 movementTarget; //this is persistent between frames

    //camera view
    float orbitSens;
    float camOrbitSlerpSpeed;

    float zoomSens;
    float zoomSlerpSpeed;
    float maxZoom;
    float minZoom;

    float minXAngle;
    float maxXAngle;

    float camCurrentX = 10; //this is persistent between frames
    float currentZoom;

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

        orbitInput.started += EnableOrbit;
        orbitInput.canceled += DisableOrbit;

        orbitInput.Enable();
        movement.Enable();
        mouseInput.Enable();
        zoomInput.Enable();

        #region getting data from scriptableObj       

        movementSpeed = cameraData.movementSpeed;
        movementSlerpSpeed = cameraData.movementSlerpSpeed;
        cameraTerrainOffset = cameraData.cameraTerrainOffset;
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
        shiftSpeedMod = cameraData.shiftSpeedMod;
        zoomSpeedMod = cameraData.zoomSpeedMod;
        #endregion

        //These track the position of something. So need to be set to the current position on enable
        camCurrentX = camTargetX.eulerAngles.x;
        movementTarget = transform.position;
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
        ViewMovement();
        MoveCamera();
    }

    void ViewMovement()
    {
        

        //for the zoom. This will make currentZoom hopefully a number between 0 and 1
        currentZoom += zoomInput.ReadValue<float>() * zoomSens;        
        currentZoom = Mathf.Clamp01(currentZoom);

        float zoomTargetPos = -(currentZoom * (maxZoom - minZoom)) + minZoom;  

        //Slerping movement for zoom
        cameraTrans.localPosition = Vector3.Lerp(cameraTrans.localPosition, new Vector3(0,0,zoomTargetPos), Time.deltaTime * zoomSlerpSpeed);


        //camera orbit 
        if (orbitEnabled) //when enabled it will read value from the mouse and set the targets positionf or the camera
        {
            float yMovement = 0;

            Vector2 mouseVec = mouseInput.ReadValue<Vector2>();
            mouseVec *= (orbitSens * Time.deltaTime);   
            yMovement = mouseVec.x;
           
            mouseVec.y = Mathf.Clamp(mouseVec.y, minXAngle-camCurrentX, maxXAngle-camCurrentX);
            camCurrentX += mouseVec.y;

            camTargetY.Rotate(0, yMovement, 0);
            camTargetX.Rotate(mouseVec.y,0,0);
        }

        //the slerp is placed here so that once it's finihsed being pressed it won't come to a sudden stop
        camPivotX.localRotation = Quaternion.Slerp(camPivotX.localRotation, camTargetX.localRotation, Time.deltaTime * camOrbitSlerpSpeed);
        camPivotY.localRotation = Quaternion.Slerp(camPivotY.localRotation, camTargetY.localRotation, Time.deltaTime * camOrbitSlerpSpeed);

        Debug.DrawRay(camPivotX.position, camPivotX.forward * -cameraTrans.position.z, Color.red);
        Debug.DrawRay(camTargetX.position, camTargetX.forward * -cameraTrans.position.z, Color.blue);
    }

    void MoveCamera()
    {
        float terrainHeight = 0;     

        Vector2 movementVec = movement.ReadValue<Vector2>();
        movementVec *= (movementSpeed * Time.deltaTime);

        //figuring out the movement modifiers
        //the more you zoom out the more speed you get
        movementVec += movementVec * (currentZoom * zoomSpeedMod);


        movementTarget += camTargetY.TransformVector(movementVec.x, 0, movementVec.y);

        //finding where the ground is from the sky for the new point
        if (Physics.Raycast(movementTarget + new Vector3(0, 1000, 0), Vector3.down * 1500, out RaycastHit hit))
        {
            terrainHeight = hit.point.y;
        }
        else
        {
            terrainHeight = 0;
        }

        terrainHeight += cameraTerrainOffset;

        //adding the terrain offset
        movementTarget = new Vector3(movementTarget.x, terrainHeight, movementTarget.z);

        //smooth damp towards location. I may change this to lerp especially when I speed up camera functionality
        
        transform.position = Vector3.Slerp(transform.position, movementTarget, movementSlerpSpeed * Time.deltaTime);

        Debug.DrawRay(transform.position, movementTarget - transform.position, Color.black);
    }

    void EnableOrbit(InputAction.CallbackContext input)
    {
        orbitEnabled = true;
    }

    void DisableOrbit(InputAction.CallbackContext input)
    {
        orbitEnabled = false;
    }
    
    void ShiftPressed(InputAction.CallbackContext input)
    {

    }
}
