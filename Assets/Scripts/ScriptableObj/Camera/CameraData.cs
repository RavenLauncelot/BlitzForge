using UnityEngine;

[CreateAssetMenu(fileName = "CameraMovement", menuName = "Data/CameraData/CameraMovement")]
public class CameraData : ScriptableObject
{
    //camera movement
    public float movementSpeed;
    public float movementSlerpSpeed;
    public float cameraTerrainOffset;

    public float shiftSpeedMod;
    public float zoomSpeedMod;

    //camera view
    public float orbitSens;
    public float camOrbitSlerpSpeed;

    public float zoomSens;
    public float zoomSlerpSpeed;
    public float maxZoom;
    public float minZoom;

    public float minXAngle;
    public float maxXAngle;
}
