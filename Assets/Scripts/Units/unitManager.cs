using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using Mono.Cecil.Cil;
using Unity.VisualScripting;
using System.Collections;

public class unitManager : MonoBehaviour
{
    Unit[] Units;

    private void Start()
    {
        Units = FindObjectsByType<Unit>(FindObjectsSortMode.None);

        StartCoroutine(LogicUpdate());
    }

    private IEnumerator LogicUpdate()
    {
        while (true)
        {
            foreach (Unit unit in Units)
            {
                if (unit.gameObject.TryGetComponent<ILogicUpdate>(out ILogicUpdate logicUpdate))
                {
                    logicUpdate.TimedLogicUpdate();
                }

                yield return new WaitForEndOfFrame();
            }
        }
    }
}
