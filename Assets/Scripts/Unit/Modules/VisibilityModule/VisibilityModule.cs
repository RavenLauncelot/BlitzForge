using UnityEngine;
using UnityEngine.Rendering;

public class VisibilityModule : UnitModule
{
    public float[] visibilityTimers;
    public bool[] visibilityMask;

    public override void CustomInit()
    {
        base.CustomInit();
        visibilityTimers = new float[8];
        visibilityMask = new bool[8];
    }

    public MeshRenderer[] meshRenderers; 

    public void IsVisibleToPlayer(bool visible)
    {
        foreach (var renderer in meshRenderers)
        {
            renderer.enabled = visible;
            renderer.material.color = Color.red;
        }
    }
}
