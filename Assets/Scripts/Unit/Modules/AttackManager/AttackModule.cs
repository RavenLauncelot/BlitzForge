using UnityEngine;

public class AttackModule : UnitModule
{
    //this is the base class for attack componennts 
    //This exists so that different types of attack components can be added
    //e.g. Artillery type aiming or normal turret aiming

    [SerializeField] private Transform aimingPos;
    public Transform AimingPos
    {
        get { return aimingPos; }
    }

    //Updates the gun's rotation
    public virtual void UpdateTurretRotation(Transform transform)
    {

    }

    //This will fire the gun
    public virtual void FireGun()
    {

    }

    //the current targets instance Id
     public Unit currentTarget;
     public Transform TargetRayCheck;

    //state booleans and counters
    public bool forcedTarget = false;
    public bool fireAtWill = true;
    public bool inLOS;
    public float reloadTimer = 0;
    public int targetSkipSearchCooldown;

    //weapon stats
    public float range = 0;
    public float damage = 0;
    public float reloadTime = 0;

}
