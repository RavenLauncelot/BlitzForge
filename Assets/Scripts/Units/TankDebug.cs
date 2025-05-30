using UnityEngine;

public class TankDebug : Unit ,IMoveable, IAttackable, IDamageable
{
    //Component list
    UnitMovement movementComp;
    UnitTargetting attackComp;

    public float setHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {      
        movementComp = GetComponent<UnitMovement>();
        attackComp = GetComponent<UnitTargetting>();

        damage = 20;
        speed = 10;
        reloadTime = 1;
        health = setHealth;
    }

    public void MoveCommand(Vector3 position)
    {
        movementComp.SetMovementTarget(position);
    }

    public void MoveCommand(Unit enemy)
    {

    }

    public void StopMovement()
    {
        movementComp.StopComponent();
    }

    public void AttackCommand(Unit unit)
    {
        attackComp.SetForcedTarget(unit);
        movementComp.SetMovementTarget(unit);
    }

    public void FireAtWill(bool enabled)
    {
        attackComp.fireAtWill = enabled;
    }

    public void Damage(float damage)
    {
        health -= damage;
    }
}
