using UnityEngine;

public class NormalTurretMod : AttackModule
{
    //This compoennt is for normal tanks with a normal moving turret as a oppose to a artillery tank.

    [SerializeField] float turretYawSpeed;
    [SerializeField] float turretPitchSpeed;

    Transform targetTrans;
    Vector3 pos;

    [SerializeField] Transform turretYawer;
    [SerializeField] Transform turretPitcher;
    [SerializeField] Transform turretPosReference;
    [SerializeField] Transform gunPosReference;

    [SerializeField] private ParticleSystem particleSys;

    public override void UpdateTurretRotation(Transform targetTransIn)
    {
        targetTrans = targetTransIn;
    }

    public void Update()
    {
        Debug.DrawRay(turretPitcher.position, turretPitcher.forward * 100);

        if (targetTrans == null)
        {
            pos = transform.position + (turretPosReference.forward * 10);
            pos.y = gunPosReference.position.y;
        }
        else
        {
            pos = targetTrans.position;
        }
       
        //rotating the y axis first (yaw)

        Quaternion turretAngleY = getTargetDirection("y", pos);
        Quaternion turretAngleX = getTargetDirection("x", pos);

        turretYawer.localRotation = Quaternion.RotateTowards(turretYawer.localRotation, turretAngleY, turretYawSpeed * Time.deltaTime);

        turretPitcher.localRotation = Quaternion.RotateTowards(turretPitcher.localRotation, turretAngleX, turretPitchSpeed * Time.deltaTime);
    }

    private Quaternion getTargetDirection(string axis, Vector3 position)
    {
        Vector3 direction = new Vector3(0, 0, 0);
        Quaternion rotationDifference;

        if (axis == "y")   //these are the axis specifcally for the gun 
        {
            direction = turretPosReference.InverseTransformPoint(position);   //for this one I had to convert it to the local position to another gameobject in the same position as the turret rotates and the local positions change
            direction.y = 0;

            //Debug.Log("Y axis target vector: " + direction);
            rotationDifference = Quaternion.LookRotation(direction, Vector3.up);   //this finds the rotation of the vector from the gun to the target the cameras looking at

            //Debug.Log("Y target local direction: " + direction + "    world direction should be teh same: " + (position - turretYawer.position));
            //Debug.Log("TargetPos: " + position);

            return rotationDifference;
        }
        else if (axis == "x")
        {
            //since this is angle of the X axis i cant just get the y positon otherwise it would just point vertical
            //I also need it so that the direction is straight ahead of the gun so it rotates on the correct axis this means i need to find the lenght of the hypotenuse of the x and z values and then point it forward ahead of the gun to get the correct rotation
            float distanceToTarget = Mathf.Sqrt(Mathf.Pow(gunPosReference.InverseTransformPoint(position).x, 2) + Mathf.Pow(gunPosReference.InverseTransformPoint(position).z, 2));       //using a^2 + b^2 = c^2  - once ive found the distance to the target im going to point it in the same direction as the gun
            direction.z = distanceToTarget;
            direction.y = gunPosReference.InverseTransformPoint(position).y;
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

    public override void FireGun()
    {
        particleSys.Play();
    }
}
