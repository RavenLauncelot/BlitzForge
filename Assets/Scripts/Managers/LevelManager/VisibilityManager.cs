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

    public UnitManager.TeamId playerTeam; 

    private void Update()
    {        
        elapsedTime = Time.deltaTime;

        //updating detection timers
        foreach (UnitModule module in managedModules)
        {
            //updating visibility timers and mesh renderers 

            VisibilityModule visModule = module as VisibilityModule;

            for (int team = 0; team < visModule.visibilityTimers.Length; team++)
            {
                //updating vistimers
                if (visModule.visibilityTimers[team] - elapsedTime < 0)
                {
                    visModule.visibilityTimers[team] = 0;
                    visModule.visibilityMask[team] = false;
                }
                else
                {
                    visModule.visibilityTimers[team] -= elapsedTime;
                }

                //This so we dont update the mesh renderer if they are on the player team
                if (visModule.TeamId == playerTeam)
                {
                    continue;
                }

                //updating mesh renderers if detected by player team. Does not update the playerTeam as they need to always be on 
                if (team == (int)UnitManager.TeamId.PlayerTeam & visModule.TeamId != UnitManager.TeamId.PlayerTeam)
                {
                    if (visModule.visibilityTimers[team] > 0)
                    {
                        visModule.IsVisibleToPlayer(true);
                    }
                    else
                    {
                        visModule.IsVisibleToPlayer(false);
                    }
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

