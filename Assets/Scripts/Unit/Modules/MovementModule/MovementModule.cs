using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.AI;

public class MovementModule : UnitModule
{
    [SerializeField] private float maxSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float stoppingDistance;

    private Vector3 currentTarget;
    private bool reachedTarget;
    [SerializeField] private NavMeshAgent agent;

    public float MaxSpeed
    {
        get { return maxSpeed; }
    }
    public float Acceleration
    {
        get { return acceleration; }
    }
    public float StoppingDistance
    {
        get { return stoppingDistance; }
    }
    public Vector3 CurrentTarget
    {
        get { return currentTarget; }
        set { currentTarget = value; }
    }
    public bool ReachedTarget
    {
        get { return reachedTarget; }
        set { reachedTarget = value; }
    }
    public NavMeshAgent Agent
    {
        get { return agent; }
        set { agent = value; }
    }
}
