using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEditor.ShaderGraph.Internal;
using Unity.Mathematics;

public class Unit : MonoBehaviour
{
    [SerializeField] private unitManager.TeamId teamId;
    public unitManager.TeamId TeamId
    {
        get { return teamId; }
    }

    [SerializeField] private MeshRenderer meshRend;

    //Basic unit stats
    public float health;
    public float speed;
    public float maxHealth;
    public float detectionRange;

    public unitManager unitManager;
 
    public void MeshRendEnabled(bool enabled)
    {
        meshRend.enabled = enabled;
    }

    public void unitInit()
    {
        meshRend = GetComponentInChildren<MeshRenderer>();
    }
}


