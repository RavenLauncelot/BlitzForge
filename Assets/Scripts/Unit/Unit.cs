using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEditor.ShaderGraph.Internal;
using Unity.Mathematics;
using Unity.VisualScripting;

public class Unit : MonoBehaviour
{
    [SerializeField] private UnitManager.TeamId teamId;
    public UnitManager.TeamId TeamId
    {
        get { return teamId; }
    }

    private int instanceId;
    [SerializeField]
    public int InstanceId
    {
        get { return instanceId; }
    }

    private bool isAlive;
    public bool IsAlive
    {
        get { return isAlive; }
    }

    [SerializeField] private SpriteRenderer selectedUnitSprite;

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

    public void InitUnit(UnitManager.TeamId team)
    {
        instanceId = gameObject.GetInstanceID();
        teamId = team;

        if (observingPos == null)
        {
            observingPos = this.transform;
        }

        if (aimingPos == null)
        {
            aimingPos = this.transform;
        }

        isAlive = true;
    }

    public UnitModule[] GetModules()
    {
        return GetComponents<UnitModule>();
    }

    public void UnitSelected(bool isSelected)
    {
        selectedUnitSprite.enabled = isSelected;
    }

    public void DestroyUnit()
    {
        if (!isAlive)
        {
            return;
        }

        //Do unit death logic here or in new class
        isAlive = false;
        instanceId = 0;

        Debug.Log(this.gameObject.name + " was destroyed");

        DestroyUnitAnim();
    }

    protected virtual void DestroyUnitAnim()
    {
        //Specific code can be written here for specific animations or effects
    }
}


