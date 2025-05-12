using UnityEngine;

public class UnitAutoLeveler : MonoBehaviour
{
    [SerializeField] private LayerMask layerMask;

    //This should be placed as a child of the unit gameobject
    //with the unit model either as a component or child of this object this script is a component of.

    void Update()
    {
        //send raycast down, find the normal of the surface, set as rotation


        Ray ray = new Ray(transform.position, -transform.parent.up);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask))
        {
            var slopeRotation = Quaternion.FromToRotation(transform.up, hit.normal);
            transform.rotation = Quaternion.Slerp(transform.rotation, slopeRotation * transform.rotation, 10 * Time.deltaTime);
        }

        Debug.DrawRay(transform.position, transform.forward*5);
        Debug.DrawRay(transform.position, transform.up*5);
    }
}
