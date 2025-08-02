using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class AiPLayer : UnitController
{
    Coroutine logicLoop;

    //Ai data
    [SerializeField] float attackDistance;

    //Capture points
    Point[] points;

    public override void StartGame()
    {
        base.StartGame();

        points = FindObjectsByType<Point>(FindObjectsSortMode.None);
        logicLoop = StartCoroutine(LogicLoop());
    }

    private IEnumerator LogicLoop()
    {
        Unit[] detectedUnits;
        Unit enemyUnit;

        CommandData attackCommand = new CommandData()
        {
            targetModule = ModuleType.AttackManager,
            commandType = "AttackCommand",
        };

        CommandData moveCommand = new CommandData()
        {
            targetModule = ModuleType.MoveManager,
            commandType = "MovementCommand"
        };

        while (true)
        {
            //I was going to update each unit slowly but since getdetected units is somewhat resource intensive im going to update the detected units once all units are updated. 
            //one unit will be updated per frame. So on a decent pc running 60fps the response time shouldn't be terrible. 
            detectedUnits = manager.GetDetectedUnits(controlledTeam); 
            foreach (Unit unit in teamUnits)
            {
                if (unit == null || !unit.IsAlive)
                {
                    continue;
                }

                enemyUnit = FindClosestUnitInRange(detectedUnits, attackDistance, unit.transform);
                

                if (enemyUnit != null)
                {
                    //attack unit
                    attackCommand.selectedUnits = new int[] { unit.InstanceId };
                    attackCommand.targettedUnits = new int[] { enemyUnit.InstanceId };

                    //MovementManager.instance.StopCommands(new int[] { unit.InstanceId });
                    manager.SendCommand(attackCommand);
                }
                else
                {
                    //go towards capture point
                    //Get capture point location
                    Vector3 Waypoint = GetPointWaypoint(unit.transform);

                    moveCommand.selectedUnits = new int[] { unit.InstanceId };
                    moveCommand.targettedArea = new Vector3[] { Waypoint };

                    manager.SendCommand(moveCommand);

                    Debug.Log("Movment command sent to AI");
                }
                
                yield return null;
            }

            //icl this was a cheap solution to units updating their position too frequently
            //Would result in them stopping right at the edge of the points when moving and stuff like that. Might be worth adjusting it so it only updated their logic once they've finished moving.
            yield return new WaitForSeconds(10f);
        }
    }

    //wil return null if no units in range
    private Unit FindClosestUnitInRange(Unit[] detectedUnits, float range, Transform position)
    {
        if (detectedUnits == null)
        {
            return null;
        }

        Unit nearbyUnit = null;

        float shortestDistance = range;
        float calculatedDistance = 0f;

        foreach (Unit unit in detectedUnits)
        {
            calculatedDistance = Vector3.Distance(unit.transform.position, position.position);
            if (calculatedDistance < range)
            {
                if (calculatedDistance < shortestDistance)
                {
                    nearbyUnit = unit;
                    shortestDistance = calculatedDistance;
                }
            }
        }

        return nearbyUnit;
    }

    //Finds the closet capture point and then returns a random position within it's capture radius
    private Vector3 GetPointWaypoint(Transform unitTransform)
    {
        Point closestPoint = null;

        float shortestDistance = 1000000f;
        float calculatedDistance = 0;

        foreach (Point point in points)
        {
            //If the point is already captured it will move on to the next point
            if (point.CapturedBy == controlledTeam && point.CapturingTeam == controlledTeam)
            {
                continue;
            }

            calculatedDistance = Vector3.Distance(point.transform.position, unitTransform.position);

            if (calculatedDistance < shortestDistance)
            {
                closestPoint = point;
                shortestDistance = calculatedDistance;
            }
        }

        if (closestPoint == null)
        {
            return unitTransform.position;
        }

        //Check if unit is already within points capture radius. If it is it will return it's current position
        if (Vector3.Distance(unitTransform.position, closestPoint.transform.position) < closestPoint.CaptureRadius)
        {
            return unitTransform.position;
        }

        //This will keep finding a point for as many as stated in parametres. It will self iterate
        return NavmeshTools.RandomPointInsideNav(closestPoint.transform.position, closestPoint.CaptureRadius, 10);
    }

    private void OnEnable()
    {
        if (gameStarted)
        {
            logicLoop = StartCoroutine(LogicLoop());
        }
        else
        {
            //Do nothing. Game hasn'tt started yet
        }
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
