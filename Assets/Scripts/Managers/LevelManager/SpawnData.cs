using Sirenix.Serialization;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnData", menuName = "Scriptable Objects/SpawnData")]
public class SpawnData : ScriptableObject
{
    public UnitManager.TeamId teamId;
    [SerializeReference]
    public UnitSpawn[] unitSpawns;

    public class UnitSpawn
    {
        public int unitAmount;
        public UnitBlueprint unitType;
    }
}


