using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "UnitBlueprint", menuName = "Scriptable Objects/UnitBlueprint")]
public class UnitBlueprint : ScriptableObject
{
    public string unitName;

    public GameObject clientUnitPrefab;

    [SerializeReference] 
    public List<ModuleDataConstructor> moduleData;
}
