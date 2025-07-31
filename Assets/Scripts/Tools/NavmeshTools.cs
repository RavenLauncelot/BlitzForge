using Sirenix.OdinInspector.Editor.TypeSearch;
using UnityEngine;
using UnityEngine.AI;

public static class NavmeshTools
{
    public static Vector3 RandomPointInsideNav(Vector3 position, float radius, int counter)
    {
        if (counter == 0)
        {
            return position;
        }

        //finding a random point within that capture point
        Vector3 randomPoint = position + (Random.insideUnitSphere * (radius * 2));
        randomPoint.y = position.y;
        NavMeshHit hit;

        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        
        else
        {
            return RandomPointInsideNav(position, radius, counter - 1);
        }
    }

    public static Vector3 RandomPointEdgeNav(Vector3 position, float radius, int counter)
    {
        if (counter == 0)
        {
            return position;
        }

        //finding a random point within that capture point
        Vector3 randomPoint = position + (Random.onUnitSphere * (radius * 2));
        randomPoint.y = position.y;
        NavMeshHit hit;

        if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
        {
            return hit.position;
        }

        else
        {
            return RandomPointInsideNav(position, radius, counter - 1);
        }
    }
}
