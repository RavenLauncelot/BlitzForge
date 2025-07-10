using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class CapturePoint : WinConditions
{
    [SerializeField] List<Point> points;

    private UnitManager.TeamId firstTeam;

    public void Start()
    {
        
    }

    //check if all the points are captured by the same team
    public void Update()
    {
        firstTeam = points[0].CapturedBy;

        if (points.All(val => val.CapturedBy == firstTeam))
        {
            winStates = firstTeam;
        }
        else
        {
            winStates = UnitManager.TeamId.None;
        }   
    }
}
