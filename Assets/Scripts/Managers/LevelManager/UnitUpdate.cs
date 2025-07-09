using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;

public class UnitUpdate : MonoBehaviour
{
    IUnitUpdate[] updateList;
    private int length;

    public void InitUnitUpdate()
    {
        List<MonoBehaviour> monoBehaviours;

        monoBehaviours = Object.FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).ToList();

        updateList = monoBehaviours.Where(val => val is IUnitUpdate).Cast<IUnitUpdate>().ToArray();
        length = updateList.Length;
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < length; i++) 
        {
            if (updateList[i] as Object == null)
            {
                RemoveInterface(i);
                continue;
            }

            updateList[i].UnitUpdate();
        }
    }

    private void RemoveInterface(int index)
    {
        updateList = updateList.Where((val, idx) => idx != index).ToArray();
    }
}

public interface IUnitUpdate
{
    public void UnitUpdate();
}
