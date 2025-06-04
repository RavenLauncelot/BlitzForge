using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "UnitBlueprint", menuName = "Scriptable Objects/UnitBlueprint")]
public class UnitBlueprint : ScriptableObject
{
    public GameObject ClientUnitPrefab;

    [SerializeReference] public List<ManagerData> ModuleData;
}
