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

    Vector3 movementTarget; //this is persistent between frames

    //camera view
    float orbitSens;
    float camOrbitSlerpSpeed;

    float zoomSens;
    float zoomSlerpSpeed;
    float maxZoom;
    float minZoom;
    float zoomTargetPos; //this basically tracks where the zoom is and uses this to make sure it's within limits

    float minXAngle;
    float maxXAngle;

    float camCurrentX = 10; //this is persistent between frames

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

        camCurrentX = camTargetX.eulerAngles.x;
        zoomTargetPos = cameraTrans.position.z;

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
        viewMovement();
        cameraMovement();
    }

    void viewMovement()
    {
        float cameraZoom = 0;

        //for the zoom 
        cameraZoom = zoomInput.ReadValue<float>();        
        cameraZoom *= zoomSens;    //because the scroll wheel is pressed like a button it doesn't need a time.delta and usually it has a value for one frame at a time
        
        //this makes sure the lerp target is within limits and maintains this target across frames. Once you stop pressing the button it should come to a smooth stop
        zoomTargetPos += Mathf.Clamp(cameraZoom, -maxZoom - zoomTargetPos, -minZoom - zoomTargetPos);  //These are the wrong way round on purpase since it's in negative numbers of z

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

    void cameraMovement()
    {
        float terrainHeight = 0;     

        

        Vector2 movementVec = movement.ReadValue<Vector2>();
        movementVec *= (movementSpeed * Time.deltaTime);

        ////this makes sure the lerp target is within limits and maintains this target across frames. Once you stop pressing the button it should come to a smooth stop
        //zoomTargetPos += Mathf.Clamp(cameraZoom, -maxZoom - zoomTargetPos, -minZoom - zoomTargetPos);  //These are the wrong way round on purpase since it's in negative numbers of z

        ////Slerping movement for zoom
        //cameraTrans.localPosition = Vector3.Lerp(cameraTrans.localPosition, new Vector3(0, 0, zoomTargetPos), Time.deltaTime * zoomSlerpSpeed);

        movementTarget += camTargetY.TransformVector(movementVec.x, 0, movementVec.y);
        
        //finding where the ground is from the sky for the new point
        RaycastHit hit;
        if (Physics.Raycast(movementTarget + new Vector3(0, 100, 0), Vector3.down * 150, out hit))
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
        Vector3 Velocity = Vector3.zero;
        transform.position = Vector3.Slerp(transform.position, movementTarget, movementSlerpSpeed * Time.deltaTime);

        Debug.DrawRay(transform.position, movementTarget - transform.position, Color.black);
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
