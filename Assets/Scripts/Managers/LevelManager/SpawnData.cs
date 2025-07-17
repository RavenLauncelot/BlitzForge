using Sirenix.Serialization;
using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class SpawnData
{
    [SerializeField]
    public Spawn[] spawns;
    public UnitManager.TeamId teamId;

    public bool AiPlayer;
}

[System.Serializable]
public class Spawn
{
    public GameObject objectToBeSpawned;
    public int amountOf;
    public Transform spawnPoint;
}


