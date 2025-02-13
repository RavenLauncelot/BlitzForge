using Pathfinding;
using System.Linq.Expressions;
using UnityEngine;

public class UnitMovement : MonoBehaviour
{
    public Transform target;

    public int currentWaypoint;

    public float speed;

    Seeker seeker;
    CharacterController controller;

    Path path;

    public bool reachedEndOfPath;

    //debug bool
    public bool setNewPath;

    void Start()
    {
        seeker = this.GetComponent<Seeker>();
        controller = this.GetComponent<CharacterController>();

        seeker.StartPath(transform.position, target.position, OnPathComplete);
    }

    public void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 0;
        }
    }

    // Update is called once per frame
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

        if (setNewPath)
        {
            seeker.StartPath(transform.position, target.transform.position, OnPathComplete);
            setNewPath = false;
        }
    }
}
