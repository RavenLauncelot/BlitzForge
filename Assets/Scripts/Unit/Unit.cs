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

    [SerializeField] private float health;
    public float Health
    {
        get { return health; }
    }

    [SerializeField] private SpriteRenderer selectedUnitSprite;

    //[SerializeField] private MeshRenderer[] meshRend;

    [Header("ray target is where other units will send rays to")]
    public Transform rayTarget;

    public void InitUnit(UnitManager.TeamId team)
    {
        instanceId = gameObject.GetInstanceID();
        teamId = team;

        if (rayTarget == null)
        {
            rayTarget = transform;
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

    public void DamageUnit(float damage)
    {
        health -= damage;
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
        SetLayer(this.gameObject, 0);

        Debug.Log(this.gameObject.name + " was destroyed");

        DestroyUnitAnim();
    }

    protected virtual void DestroyUnitAnim()
    {
        //Specific code can be written here for specific animations or effects
    }

    private void SetLayer(GameObject parent, int layer)
    {
        parent.layer = layer;
        foreach (Transform child in parent.transform)
        {
            child.gameObject.layer = layer;

            Transform _HasChildren = child.GetComponentInChildren<Transform>();
            if (_HasChildren != null)
            {
                SetLayer(child.gameObject, layer);
            }
        }
    }
}


