using UnityEngine;

public interface IAttackable
{
    public void AttackCommand(Unit unit);

    public void FireAtWill(bool enabled);
}
