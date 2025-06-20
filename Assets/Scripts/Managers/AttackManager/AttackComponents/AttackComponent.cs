using UnityEngine;

public class AttackComponent : MonoBehaviour
{
    //this is the base class for attack componennts 
    //This exists so that different types of attack components can be added
    //e.g. Artillery type aiming or normal turret aiming
    
    //Updates the gun's rotation
    public virtual void UpdateTurretRotation(Vector3 targetLocation)
    {

    }

    //This will fire the gun
    public virtual void FireGun()
    {

    }
}
