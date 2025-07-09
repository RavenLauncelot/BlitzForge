using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using static UnitManager;
using Unity.VisualScripting;

public class DetectionManager : ModuleManager
{
    public LayerMask unitLayer;

    private Coroutine updateLoop;

    [SerializeField] private int frameSkip;

    public void Start()
    {
        StartCoroutine(UpdateLoop());
    }

    private void OnEnable()
    {
        updateLoop = StartCoroutine(UpdateLoop());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void Update()
    {
        //per frame updates for:
        //detectionTimers
        //visibility bitmasks.

        foreach (UnitModule unitModule in managedModules)
        {
            DetectionModule detectionModule = unitModule as DetectionModule;
            int unitDataIndex = manager.unitDataIndexLookup[unitModule.InstanceId];

            //this updates all the visibility timers for this team specifically 
            //once a timer reaches zero it will no longer be detected by that team (team enum is represented as a int/index)
            for (int i = 0; i < detectionModule.DetectedTimers.Length; i++)
            {
                if (detectionModule.DetectedTimers[i] < Time.deltaTime)
                {
                    detectionModule.DetectedTimers[i] = 0;
                    manager.unitData[unitDataIndex].teamVisibility[i] = false;
                }
                else
                {
                    detectionModule.DetectedTimers[i] -= Time.deltaTime;
                }
            }
        }
    }

    private IEnumerator UpdateLoop()
    {
        List<Unit> tempList = new List<Unit>();

        while (true)
        {
            foreach (UnitModule unitModule in managedModules)
            {
                DetectionModule detectionData = unitModule as DetectionModule;

                tempList = DetectEnemies(detectionData.ObservPos.position, detectionData);

                //now we set the unitdata for the detected enemies to say tehy are detected by the team that detected them
                foreach (Unit detected in tempList)
                {
                    levelManager.SetDetected(detected.InstanceId, manager.managedTeam, detectionData.DetectionTime);
                }

                for (int frame = 0; frame < frameSkip; frame++)
                {
                    yield return null;
                }             
            }

            //this is so unity doesn't crash when there are zero units lmoa
            yield return null;
        }
    }

    private List<Unit> DetectEnemies(Vector3 observPos, DetectionModule detectionModule)
    {
        List<Unit> detectedUnits = new List<Unit>();

        Collider[] detected = new Collider[100];

        int collisions = Physics.OverlapSphereNonAlloc(observPos, detectionModule.DetectionRange, detected, unitLayer);
        for (int i = 0; i < collisions; i++)
        {
            //checking if there is a unit script inside the dectected collider
            if (detected[i].transform.root.TryGetComponent<Unit>(out Unit unitCode))
            {
                //if the team id is the same as the detecting unit it will skip
                if (unitCode.TeamId == manager.managedTeam)
                {
                    continue;
                }
            }
            //no unit script skip
            else
            {
                continue;
            }



            //checking unit is within line of sight 
            Ray ray = new Ray(observPos, unitCode.rayTarget.position - observPos);
            RaycastHit hit;
            Debug.DrawRay(observPos, unitCode.rayTarget.position - observPos, Color.blue, 4f);

            if (Physics.Raycast(ray, out hit, detectionModule.DetectionRange))
            {
                if (hit.collider.transform.root.TryGetComponent<Unit>(out Unit unit) && unit.InstanceId == unitCode.InstanceId)
                {
                    detectedUnits.Add(unitCode);
                }

                else
                {
                    //try again
                }
            }
            else
            {
                Debug.Log("Raycast blocked or didn't reach" + gameObject.name);
            }
        }

        return detectedUnits;
    }
}

//Since detection will most likely be used a lot it is directly stored in the UnitData array
//this includes the DetectedTimers array and teamVisibility
[System.Serializable]
public class DetectionData : ModuleData
{
    public DetectionData()
    {
        moduleType = "DetectionManager";
    }

    public override ModuleData Clone()
    {
        return new DetectionData()
        {
            moduleType = moduleType,
            detectionTime = detectionTime,
            detectionRange = detectionRange,
        };
    }

    public float detectionRange;
    public float detectionTime;

    public float[] detectedTimers = new float[8];
}
