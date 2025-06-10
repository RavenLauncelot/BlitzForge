using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEditor.ShaderGraph.Internal;
using Unity.Mathematics;

public class Unit : MonoBehaviour
{
    [SerializeField] private UnitManager.TeamId teamId;
    public UnitManager.TeamId TeamId
    {
        get { return teamId; }
        set { teamId = value; }
    }

    [SerializeField] private MeshRenderer meshRend;

    [Header("Observing pos is the positon it will send raycasts to detect enemies \n aimingPos is the position it will fire from \n ray target is where other units will send rays to")]
    public Transform observingPos;
    public Transform aimingPos;
    public Transform rayTarget;

    public int instanceId;

    public UnitManager unitManager;
 
    public void MeshRendEnabled(bool enabled)
    {
        meshRend.enabled = enabled;
    }

    public void initUnit()
    {
        meshRend = GetComponentInChildren<MeshRenderer>();

        instanceId = gameObject.GetInstanceID();

        if (observingPos == null)
        {
            observingPos = this.transform;
        }

        if (aimingPos == null)
        {
            aimingPos = this.transform;
        }
    }
}


