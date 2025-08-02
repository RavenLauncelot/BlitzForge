using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using static UnitManager;

public class VisibilityManager : ModuleManager
{
    public static VisibilityManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    private float elapsedTime;

    Coroutine updateLoop;

    public UnitManager.TeamId playerTeam;

    public VisibilityModule[] visibilityModules;
    public Dictionary<int, VisibilityModule> visModuleIdLookup;

    public override void InitModuleManager()
    {
        base.InitModuleManager();

        visibilityModules = managedModules.Cast<VisibilityModule>().ToArray();

        visModuleIdLookup = visibilityModules.ToDictionary(val => val.InstanceId, val => val);
    }

    public override void StartModuleManager()
    {
        base.StartModuleManager();
    }

    public override void UnregisterModule(int id)
    {
        visibilityModules = visibilityModules.Where(val => val.InstanceId != id).ToArray();
        managedModules = managedModules.Where(val => val.InstanceId != id).ToArray();
    }

    private void Update()
    {        
        elapsedTime = Time.deltaTime;

        //updating detection timers
        foreach (VisibilityModule visModule in visibilityModules)
        {
            //updating visibility timers and mesh renderers 

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
        if (visModuleIdLookup.TryGetValue(instanceId, out VisibilityModule visModule))
        {
            visModule.visibilityTimers[(int)detectedBy] = detectionTime;
            visModule.visibilityMask[(int)detectedBy] = true;
        }
    }

    public bool IsTargetDetected(int targetId, TeamId detectedBy, float timerMinimum)
    {
        VisibilityModule visModule;
        if (VisibilityManager.instance.visModuleIdLookup.TryGetValue(targetId, out visModule))
        {
            //This check if the timer is above 1.
            //When it would check the mask it would result in the units flickering. 
            //as the timers would not be refreshed. 
            //I was going to refresh the timers in here but that could potentially mean units that are not within LOS would stay detected.
            if (visModule.visibilityTimers[(int)detectedBy] > timerMinimum)
            {
                return true;
            }
        }

        return false;
    }
}

