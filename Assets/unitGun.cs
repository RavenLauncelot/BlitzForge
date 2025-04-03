using NUnit.Framework.Interfaces;
using Unity.VisualScripting;
using UnityEngine;

public class unitGun : MonoBehaviour
{
    public float fireRate;
    public float damage;
    public Vector2 pitchRange;

    public Transform pivotX;
    public Transform pivotY;
    public float pivotZSpeed;
    public float pivotYSpeed;

    private float fireCounter = 0;
    private bool onTarget = false;
    private Transform target;

    public UnitTargetting targettingComp;

    private void Start()
    {
        if (targettingComp == null)
        {
            Debug.Log("Targetting component is null: " + gameObject.transform.parent.parent.parent.name);
        }
    }

    private void Update()
    {
        target = targettingComp.GetTargetTrans();
        fireCounter += Time.deltaTime;

        //timer is over fire next projectile
        if (fireCounter >= fireRate || onTarget)
        {
            fireCounter = 0;

            Fire();
        }
        else
        {
            
        }

        //if not on target aim towards target
        if (!onTarget)
        {
            AimTowards();
        }
        else
        {
            Ray onTargetCheck = new Ray(pivotX.position, targettingComp.currentTarget.transform.position);

            if (!Physics.Raycast(onTargetCheck))
            {
                onTarget = false;
            }
            else if (Physics.Raycast(onTargetCheck, out RaycastHit hit))
            {
                if (hit.collider.GetComponent<Unit>() != targettingComp.currentTarget)
                {
                    onTarget = false;
                }
            }
        }
    }

    private void Fire()
    {

    }

    private void AimTowards()
    {
        if (targettingComp.currentTarget == null)
        {
            return;
        }

        pivotY.localRotation = Quaternion.RotateTowards(pivotY.localRotation, getTargetDirection("y"), pivotYSpeed * Time.deltaTime);

        //check if ray hits target
        Ray onTargetCheck = new Ray(pivotX.position, targettingComp.currentTarget.transform.position);

        if (Physics.Raycast(onTargetCheck, out RaycastHit hit))
        {
            if (hit.collider.GetComponent<Unit>() == targettingComp.currentTarget)
            {
                onTarget = true;
            }
        }
    }

    private Quaternion getTargetDirection(string axis)
    {
        Vector3 direction = new Vector3(0, 0, 0);
        Quaternion rotationDifference;

        Vector3 targetPos = target.position;

        if (axis == "y")   //these are the axis specifcally for the gun 
        {
            direction = pivotY.InverseTransformPoint(targetPos);   //for this one I had to convert it to the local position to another gameobject in the same position as the turret rotates and the local positions change
            direction.y = 0;

            //Debug.Log("Y axis target vector: " + direction);
            rotationDifference = Quaternion.LookRotation(direction, Vector3.up);   //this finds the rotation of the vector from the gun to the target the cameras looking at
            return rotationDifference;
        }
        else if (axis == "x")
        {
            //since this is angle of the X axis i cant just get the y positon otherwise it would just point vertical
            //I also need it so that the direction is straight ahead of the gun so it rotates on the correct axis this means i need to find the lenght of the hypotenuse of the x and z values and then point it forward ahead of the gun to get the correct rotation
            float distanceToTarget = Mathf.Sqrt(Mathf.Pow(pivotY.InverseTransformPoint(targetPos).x, 2) + Mathf.Pow(pivotY.InverseTransformPoint(targetPos).z, 2));       //using a^2 + b^2 = c^2  - once ive found the distance to the target im going to point it in the same direction as the gun
            direction.z = distanceToTarget;
            direction.y = pivotY.InverseTransformPoint(targetPos).y;
            direction.x = 0;

            //Debug.Log("X axis target vector: " + direction);

            rotationDifference = Quaternion.LookRotation(direction, Vector3.up);
            return rotationDifference;
        }
        else
        {
            return Quaternion.Euler(0, 0, 0); //error
        }
    }
}
