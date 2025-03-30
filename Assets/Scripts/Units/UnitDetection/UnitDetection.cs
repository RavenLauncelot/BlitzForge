using System.Collections;
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;
using UnityEngine;

public class UnitDetection : MonoBehaviour
{
    public bool canSearch;
    public float searchRadius;
    public LayerMask unitLayerMask;

    [SerializeField] public bool isDetected;
    private MeshRenderer rend;

    public void Start()
    {
        StartCoroutine(SearchArea());

        //team A and B
        isDetected = false;

        rend = GetComponentInChildren<MeshRenderer>();
        rend.enabled = false;
    }

    //debug
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, searchRadius);
    }

    public void searchArea()
    {
        Collider[] inRange = Physics.OverlapSphere(transform.position, searchRadius, unitLayerMask);


        Debug.Log(inRange[0]);

        //Unit is within range lets see if they can be spotted (they arent behind a wall)
        foreach (Collider c in inRange)
        {
            if (c.transform.parent.TryGetComponent<UnitDetection>(out UnitDetection unitDetect) == true)
            {
                Debug.Log("Discovered Unit: " + c.gameObject.name);
            }

            else
            {
                Debug.Log("Not discoverable Unit name: " + c.gameObject.name);
                break;
            }

            Ray ray = new Ray(transform.position, c.transform.position - transform.position);
            Debug.DrawRay(transform.position, c.transform.position - transform.position, Color.red, 2);

            
            
            if (Physics.Raycast(ray, out RaycastHit hit, unitLayerMask))
            {
                if (hit.collider.gameObject == c.gameObject)
                {
                    Debug.Log("Enemy spotted gameobject name: " + c.gameObject.name);
                    unitDetect.makeVisible();
                }               
            }

            else
            {
                Debug.Log("Raycast was blocked");
                break;
            }         
        }
    }

    //if hasnt been detected will go back to being undetected
    public void detectionRefresh()
    {

    }

    public IEnumerator SearchArea()
    {
        while (true)
        {
            if (canSearch)
            {
                searchArea();
                yield return new WaitForSeconds(2);
            }

            yield return new WaitForEndOfFrame();
        }
    }

    public void makeVisible()
    {
        rend.enabled = true;
        isDetected = true;
    }
}

