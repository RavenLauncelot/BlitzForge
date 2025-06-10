using UnityEngine;

public class UnitAttackModule : UnitModule
{
    //This will be the class attached to units to define whether or not they are attacking Units.
    //This will contain some data for the attackmanager to use. but will not be processed inside this object. This object is mainly for animation 

    //This is the scrtipable object which contains all relevant information for the manaher
    //only used once
    public AttackData attackData;

    //This points the turret at a target
    public TurretAutoAim turretAiming;

    //this will fire the projectile
    public ParticleSystem gunParticle;

    //public override ManagerData GetManagerData()
    //{
    //    if (attackModuleData == null)
    //    {
    //        Debug.LogWarning("Attack module data missing");

    //        return new AttackData();
    //    }

    //    return new AttackData()
    //    {
    //        damage = attackModuleData.damage,
    //        range = attackModuleData.range,
    //        reloadTime = attackModuleData.reloadTime
    //    };
    //}


    private void Start()
    {
        if (turretAiming == null)
        {
            Debug.LogWarning("Missing turret auto aim for attack module");
        }
    }

    //Updates the gun's rotation
    public void UpdateAttackModule(Vector3 targetLocation)
    {
        turretAiming.SetTargetPos(targetLocation);
        Debug.Log("I should be updating");

    }

    public void FireGun()
    {
        gunParticle.Play();
    }
}
