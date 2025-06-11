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

    private int instanceId;
    public int InstanceId
    {
        get { return instanceId; }
    }

    [SerializeField] private MeshRenderer[] meshRend;

    [Header("Observing pos is the positon it will send raycasts to detect enemies")]
    public Transform observingPos;
    [Header("aimingPos is the position it will fire from")]
    public Transform aimingPos;
    [Header("ray target is where other units will send rays to")]
    public Transform rayTarget;
 
    public void MeshRendEnabled(bool enabled)
    {
        foreach(MeshRenderer rend in meshRend)
        {
            rend.enabled = enabled;
        }
    }

    public void InitUnit()
    {
        //meshRend = GetComponentsInChildren<MeshRenderer>();

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


