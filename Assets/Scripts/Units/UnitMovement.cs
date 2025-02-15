using Pathfinding;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class UnitMovement : UnitComponent
{
    private Command currentCommand;
    private Seeker seeker;
    private CharacterController controller;

    private Path path;

    private Vector3 currentTarget;

    //pathfinding
    [SerializeField] bool reachedEndOfPath;
    [SerializeField] int currentWaypoint;
    [SerializeField] float speed;

    private void Start()
    {
        seeker = GetComponent<Seeker>();
        controller = GetComponent<CharacterController>();
    }

    public override void CommandTick()   //checks for new commands and acts on them
    {
        base.CommandTick();

        if (unit.GetCommand() == null)
        {
            currentCommand = null;
            return;
        }

        else if (reachedEndOfPath)
        {
            unit.CommandCompleted();
            reachedEndOfPath = false;
            path = null;
            return;
        }

        else if (currentCommand == unit.GetCommand())
        {
            return;
        }

        else  //different command detected
        {
            currentCommand = unit.GetCommand();

            if (currentCommand.commandType == CommandTypes.Move)
            {
                
            }
        }
    }

    public override void NavTick()
    {
        base.NavTick();

        if (currentCommand.commandType == CommandTypes.Move && path == null)
        {           
            seeker.StartPath(transform.position, currentCommand.positionData, OnPathComplete);
        }

        else if (currentCommand.commandType == CommandTypes.Attack)
        {
            //do stuff for attack
        }
    }

    public void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
            unit.CommandCompleted();
        }
    }

    void Update()
    {        

        if (path == null)
        {
            return;
        }

        reachedEndOfPath = false;
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
                    reachedEndOfPath = true;
                    break;
                }
            }

            else
            {
                break;
                //no change in direction
            }
        }

        Vector3 dir = (path.vectorPath[currentWaypoint] - transform.position).normalized;
        Vector3 velocity = dir * speed;
        controller.Move(velocity);
    }
}
