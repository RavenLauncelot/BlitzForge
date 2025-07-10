using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class VisibilityManager : ModuleManager
{
    private float elapsedTime;
    private Stopwatch stopWatch;

    Coroutine updateLoop;

    private void Update()
    {        
        elapsedTime = Time.deltaTime;

        //updating detection timers
        foreach (UnitModule module in managedModules)
        {
            VisibilityModule visModule = module as VisibilityModule;

            for (int team = 0; team < visModule.visibilityTimers.Length; team++)
            {
                //Version1 
                if (visModule.visibilityTimers[team] - elapsedTime < 0)
                {
                    visModule.visibilityTimers[team] = 0;
                    visModule.visibilityMask[team] = false;
                }
                else
                {
                    visModule.visibilityTimers[team] -= elapsedTime;
                }
            }
        }   
    }

    public void SetDetected(int instanceId, float detectionTime, UnitManager.TeamId detectedBy)
    {
        if (moduleIdLookup.TryGetValue(instanceId, out UnitModule module))
        {
            VisibilityModule visModule = module as VisibilityModule;

            visModule.visibilityTimers[(int)detectedBy] = detectionTime;
            visModule.visibilityMask[(int)detectedBy] = true;
        }
    }
}

