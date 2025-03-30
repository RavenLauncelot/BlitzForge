using UnityEngine;

public interface IMoveable
{
    public void MoveCommand(Vector3 position);

    public void MoveCommand(Unit enemy);

    public void StopMovement();
}
