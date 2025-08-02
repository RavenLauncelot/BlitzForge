using NUnit.Framework;
using Sirenix.OdinInspector.Editor.TypeSearch;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using Unity.VisualScripting.FullSerializer.Internal;

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

    public static Vector3[] EvenSpacing(int positions, Vector3 origin, float spacing)
    {
        List<Vector3> positionList = new List<Vector3>();

        if (positions < 1)
        {
            Debug.LogWarning("NavmeshTools.EvenSpacing error positions 0");
        }

        if (positions == 1)
        {
            return new Vector3[] { origin };
        }

        positionList.Add(origin);
        positions--;

        return GetSquarePoints(2, positions, spacing, origin, positionList).ToArray();
    }

    private static List<Vector3> GetSquarePoints(int spaceCount, int remainingPositions, float spacing, Vector3 origin, List<Vector3> positionList)
    {
        if (spaceCount > 100)
        {
            Debug.LogWarning("NavmeshTools.EvenSpacing No positions available aborting");
            
            return positionList;
        }

        float squareSize = spacing * spaceCount;
        Vector3 currentPoint = new Vector3(origin.x - (squareSize / 2), origin.y, origin.z - (squareSize / 2));

        NavMeshHit navMeshHit = new NavMeshHit();

        Vector3 Side = Vector3.zero;

        for (int direction = 0; direction < 4; direction++)
        {         
            switch (direction)
            {
                case 0:
                    Side = Vector3.right; break;
                case 1: 
                    Side = Vector3.forward; break;
                case 2:
                    Side = Vector3.left; break;
                case 3:
                    Side = Vector3.back; break;
            }

            for (int sides = 0; sides < spaceCount; sides++)
            {
                currentPoint += Side * spacing;

                //If it can't find a position on the navmesh it will skip
                Ray ray = new Ray(currentPoint + (Vector3.up * 500), Vector3.down);
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
                {
                    if (NavMesh.SamplePosition(hit.point, out navMeshHit, 1f, NavMesh.AllAreas))
                    {
                        positionList.Add(navMeshHit.position);

                        remainingPositions--;
                    }
                }

                if (remainingPositions == 0)
                {
                    return positionList;
                }
            }
        }

        //remaingPositions still remaing re iterating
        return GetSquarePoints(spaceCount + 2, remainingPositions, spacing, origin, positionList);
    }
}
