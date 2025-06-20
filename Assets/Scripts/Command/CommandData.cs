using UnityEngine;
using UnityEngine.Rendering;

[System.Serializable]
public class CommandData
{
    public string targetModule;
    public string commandType;

    public int[] selectedUnits;

    public Vector3[] targettedArea;
    public int[] targettedUnits;
}
