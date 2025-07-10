using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEditor;

public class MeshRendManager : MonoBehaviour
{
    [SerializeField] VisibilityManager visibilityManager;
    [SerializeField] LevelManager levelManager;

    [SerializeField] UnitManager.TeamId playerTeam;

    List<int> visibleUnits = new List<int>();
    
}
