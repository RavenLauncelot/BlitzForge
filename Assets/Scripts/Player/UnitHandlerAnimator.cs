using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class UnitHandlerAnimator : MonoBehaviour
{
    UnitHandler unitHand;
    LineRenderer lineRenderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        unitHand = GetComponent<UnitHandler>();
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = 5;
    }

    // Update is called once per frame
    void Update()
    {
        if (unitHand.IsSelecting)
        {
            DrawBoxCollider();
        }

        else if (unitHand.IsTargeting) 
        {
            
        }

        //Cursor on ground maybe?
        else
        {
            lineRenderer.enabled = false;
        }
    }

    private void DrawBoxCollider()
    {
        Vector3[] points = new Vector3[5];
        Vector3[] lowerBounds = unitHand.GetLowerBounds();

        //This is just optimasation making sure it only checks an amount it really needs to 
        float checkingRange = Mathf.Abs(lowerBounds[0].y - lowerBounds[2].y);

        //index 1 and 2 are aproximated 
        if (NavMesh.SamplePosition(lowerBounds[1], out NavMeshHit hit, checkingRange, NavMesh.AllAreas))
        {
            lowerBounds[1] = hit.position;
        }

        else
        {
            Debug.Log("HUh");
        }

        if (NavMesh.SamplePosition(lowerBounds[3], out NavMeshHit hitAgainnn, checkingRange, NavMesh.AllAreas))
        {
            lowerBounds[3] = hitAgainnn.position;
        }

        //points needs to go back round to the beginning so appending to new list.
        for (int i = 0; i < 4; i++)
        {
            points[i] = lowerBounds[i] + new Vector3(0,1,0);
        }
        points[4] = points[0];

        lineRenderer.SetPositions(points);
        lineRenderer.enabled = true;
    }
}
