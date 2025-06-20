using UnityEngine;

public class UnitAutoLeveler : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;
    private Transform transformRoot;

    //This should be placed as a child of the unit gameobject
    //with the unit model either as a component or child of this object this script is a component of.

    [SerializeField] private float smoothing = 10f;

    private void Start()
    {
        transformRoot = gameObject.transform.root;
    }

    void Update()
    {
        Ray ray = new Ray(transformRoot.position, -transformRoot.up);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            Vector3 groundNormal = hit.normal;

            // Keep current forward direction projected onto the new surface
            Vector3 forwardProjected = Vector3.ProjectOnPlane(transformRoot.forward, groundNormal).normalized;

            // Construct target rotation using the projected forward and the surface normal as up
            Quaternion targetRotation = Quaternion.LookRotation(forwardProjected, groundNormal);

            // Smooth the rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothing * Time.deltaTime);
        }

        Debug.DrawRay(transform.position, transform.forward * 2, Color.blue);
        Debug.DrawRay(transform.position, transform.up * 2, Color.green);
    }
}

