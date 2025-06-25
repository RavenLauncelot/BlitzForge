using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;

public class Point : MonoBehaviour
{
    [SerializeField] private float captureTime;
    [SerializeField] private float captureRadius;
    [SerializeField] private LayerMask unitLayer;

    [ShowInInspector] private UnitManager.TeamId capturingTeam;
    public UnitManager.TeamId CapturingTeam
    {
        get { return capturingTeam; }
    }
    [ShowInInspector] private float captureProgress;
    public float CaptureProgress
    {
        get { return captureProgress; }
    }

    [ShowInInspector] private bool isCaptured;
    public bool IsCaptured
    {
        get { return isCaptured; }
    }
    [ShowInInspector] private UnitManager.TeamId capturedBy;
    public UnitManager.TeamId CapturedBy
    {
        get { return capturedBy; }
    }


    //frequently used variables
    Collider[] colliders;
    public UnitManager.TeamId firstUnitTeam;

    private void Start()
    {
        capturedBy = UnitManager.TeamId.None;
        capturingTeam = UnitManager.TeamId.None;

        colliders = new Collider[100];
    }

    public void Update()
    {
        colliders = new Collider[100];
        if (Physics.OverlapSphereNonAlloc(transform.position, captureRadius, colliders, unitLayer) > 0)
        {
            Debug.Log("Meow");

            firstUnitTeam = colliders[0].transform.root.GetComponent<Unit>().TeamId;
            foreach (Collider col in colliders)
            {
                if (col == null) { continue; }

                if (col.transform.root.GetComponent<Unit>().TeamId != firstUnitTeam)
                {
                    Debug.Log("Contested capture point " + gameObject.name);
                    isCaptured = false;
                    capturedBy = UnitManager.TeamId.None;
                    capturingTeam = UnitManager.TeamId.None;
                    captureProgress = 0;    
                    return;
                }
            }

            //reseting capture progress if different team
            if (capturingTeam != firstUnitTeam)
            {
                captureProgress = 0f;
                isCaptured = false;
                capturedBy = UnitManager.TeamId.None;
                capturingTeam = firstUnitTeam;
            }

            //same team as before adding time on and checking
            else
            {
                if (captureProgress + Time.deltaTime > captureTime)
                {
                    captureProgress = captureTime;
                    isCaptured = true;
                    capturedBy = capturingTeam;
                    Debug.Log("Capture point " + gameObject.name + " captured by team " + capturingTeam);
                }

                else
                {
                    captureProgress += Time.deltaTime;
                }
            }
        }

        else if (!isCaptured)
        {
            captureProgress = 0f;
            capturingTeam = UnitManager.TeamId.None;
            Debug.Log("Not captured");
        }

        else
        {
            //do nothing this point is captured already
            Debug.Log("Bruh");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, captureRadius);
    }
}
