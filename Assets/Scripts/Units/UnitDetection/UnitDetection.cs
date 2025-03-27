using UnityEngine;
using Unity.Physics;

public class UnitDetection : MonoBehaviour
{
    bool canSearch;
    float searchRadius;
    
    public void searchArea()
    {
        Collider[] inRange = Physics.OverlapSphere(transform.position, searchRadius);

        //Unit is within range lets see if they can be spotted (they arent behind a wall)
        foreach (Collider c in inRange)
        {
            if (Physics.Raycast())
        }
    }

    //if hasnt been detected will go back to being undetected
    public void detectionRefresh()
    {

    }
}

public struct isDetected
{
    
}
