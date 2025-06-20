using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;

[CreateAssetMenu(fileName = "UnitBlueprint", menuName = "Scriptable Objects/UnitBlueprint")]
public class UnitBlueprint : ScriptableObject
{
    public string unitName;

    public GameObject unitPrefab;

    public BasicUnitData unitData;

    [SerializeReference] 
    public List<ModuleData> moduleData;
}
