using Pathfinding;
using UnityEngine;

public class UnitMovement : UnitComponent
{
    private Seeker seeker;
    private CharacterController controller;

    private Path path;

    private Vector3 currentTarget;

    //pathfinding
    [SerializeField] public bool movementDone;
    [SerializeField] int currentWaypoint;
    [SerializeField] int totalWaypoint;
    [SerializeField] float speed;

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        controller = GetComponent<CharacterController>();

        componentType = UnitComponents.UnitMovement;
    }

    public override void SetMovementTarget(Vector3 position)
    {
        base.SetMovementTarget(position);

        currentTarget = position;

        seeker.StartPath(transform.position, currentTarget, OnPathComplete);

        movementDone = false;
    }

    public void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
            return;
        }
    }

    void Update()
    {        
        if (path == null)
        {
            return;
        }

        movementDone = false;
        float distanceToWaypoint;

        while (true)
        {
            distanceToWaypoint = Vector3.Distance(transform.position, path.vectorPath[currentWaypoint]);

            if (distanceToWaypoint < 2)
            {
                if (currentWaypoint + 1 < path.vectorPath.Count)
                {
                    currentWaypoint++;
                    //reached a waypoint moving on to next waypoint
                }
                else
                {
                    movementDone = true;
                    path = null;
                    break;
                }
            }

            else
            {
                //no change in direction
                break;
            }
        }

        Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        Vector3 velocity = dir * speed;
        controller.Move(velocity);

        totalWaypoint = path.vectorPath.Count;
    }
}
