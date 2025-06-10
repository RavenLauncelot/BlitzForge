using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "SpawnData", menuName = "Scriptable Objects/SpawnData")]
public class SpawnData : ScriptableObject
{
    public UnitManager.TeamId teamId;
    [SerializeReference]
    public int[] unitAmount;
    [SerializeReference]
    public UnitBlueprint[] unitBlueprint;
}


