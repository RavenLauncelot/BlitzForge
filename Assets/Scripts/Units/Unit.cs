using NUnit.Framework;
using UnityEngine;
using Pathfinding;

public class Unit : MonoBehaviour
{
    private float health;
    private float speed;
    
    private Seeker seeker;

    private Path currentPath;

    private bool FindPath(out Path p, Vector3 position)
    {
        p = seeker.StartPath(transform.position, position);

        if (!p.error)
        {
            return true;
        }

        return false;
    }

    private void navCheck()
    {
        if (currentPath == null)
        {
            return;
        }


    }
}
