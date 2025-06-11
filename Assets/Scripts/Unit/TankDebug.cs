using UnityEngine;

public class TankDebug : Unit ,IAttackUpdate
{
    //Component list
    UnitAttackModule attackComp;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {      
        attackComp = GetComponent<UnitAttackModule>();
    }

    public void MoveCommand(Vector3 position)
    {
        //movementComp.SetMovementTarget(position);
    }

    public void MoveCommand(Unit enemy)
    {

    }

    public void StopMovement()
    {
        //movementComp.StopComponent();
    }

    //Attack Updates
    public void AttackVisualUpdate(Vector3 p)
    {
        attackComp.UpdateAttackModule(p);
    }

    public void AttackFireProjectile()
    {
        attackComp.FireGun();
    }
}
