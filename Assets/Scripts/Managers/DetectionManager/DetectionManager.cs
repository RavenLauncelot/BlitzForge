using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;

public class DetectionManager : ModuleManager
{
    public LayerMask unitLayer;

    public void Start()
    {
        moduleType = ModuleKind.DetectionModule;
        unitIds = GetIds(ModuleManager.ModuleKind.DetectionModule);

        StartCoroutine(DetectionUpdate());
    }
        
    private IEnumerator DetectionUpdate()
    {
        List<Unit> tempList = new List<Unit>();

        while (true)
        {
            for (int i = 0; i < unitIds.Length; i++)
            {
                int unitIndex = manager.unitDataIndexLookup[unitIds[i]];
                DetectionData detectionData = manager.GetModuleData(unitIds[i], ModuleManager.ModuleKind.DetectionModule) as DetectionData;

                tempList = DetectEnemies(manager.unitData[unitIndex].observingPos.position, detectionData);

                //now we set the unitdata for the detected enemies to say tehy are detected by the team that detected them
                foreach (Unit detected in tempList)
                {
                    levelManager.setDetected(detected.InstanceId, manager.managedTeam, detectionData.detectionTime);
                }

                yield return new WaitForEndOfFrame();
            }

            //this is so unity doesn't crash when there are zero units lmoa
            yield return new WaitForEndOfFrame();
        }
    }

    private List<Unit> DetectEnemies(Vector3 observPos, DetectionData detectionData)
    {
        List<Unit> detectedUnits = new List<Unit>();

        Collider[] detected = new Collider[20];

        int collisions = Physics.OverlapSphereNonAlloc(observPos, detectionData.detectionRange, detected, unitLayer);
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

            if (Physics.Raycast(ray, out hit, detectionData.detectionRange))
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
public class DetectionData : ModuleData
{
    public DetectionData()
    {
        moduleType = ModuleManager.ModuleKind.DetectionModule;
    }

    public float detectionRange;
    public float detectionTime;
    public Transform detectionFrom;

    public float detectionTimer;
}

[CreateAssetMenu(fileName = "DetectionModuleData", menuName = "Scriptable Objects/ModuleData/DetectionModuleData")]
public class DetectionDataConstructor : ModuleDataConstructor
{
    public float detectionRange;
    
    public float detectionTime;

    public override ModuleData GetNewData()
    {
        base.GetNewData();

        return new DetectionData()
        {
            detectionRange = detectionRange,
            detectionTime = detectionTime
        };
    }
}
