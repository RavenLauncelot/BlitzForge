using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : UnitComponent
{
    private NavMeshAgent agent;
    private Vector3 currentTarget;

    //pathfinding
    [SerializeField] public bool movementDone;
    [SerializeField] int currentWaypoint;
    [SerializeField] int debugTotalWaypoint;
    [SerializeField] float speed;

    private void Start()
    {
        componentType = UnitComponents.UnitMovement;

        agent = GetComponent<NavMeshAgent>();
    }

    public void SetMovementTarget(Vector3 position)
    {
        movementDone = true;
        agent.isStopped = false;
        currentTarget = position;
        
        agent.SetDestination(position);
    }

    public void SetMovementTarget(Unit target)
    {
        //we'll do this later
    }

    public override void StopComponent()
    {
        agent.isStopped = true;
    }

    void Update()
    {        
        if (movementDone) { return; }
        
        if (Vector3.Distance(transform.position ,currentTarget) <= agent.stoppingDistance)
        {
            movementDone = true;
        }
    }
}
