using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class DetectionModule : UnitModule
{
    [SerializeField] private float detectionRange;
    [SerializeField] private float detectionTime;
    [SerializeField] private float[] detectedTimers = new float[8];
    [SerializeField] private Transform observPos;

    public float DetectionRange
    {
        get { return detectionRange; }
    }
    public float DetectionTime
    {
        get { return detectionTime; }
    }
    public float[] DetectedTimers
    {
        get { return detectedTimers; }
        set { detectedTimers = value; }
    }
    public Transform ObservPos
    {
        get { return observPos; }
    }
}
