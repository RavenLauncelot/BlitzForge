using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Collections;
using static UnitManager;
using Unity.VisualScripting;
using System.Linq;

public class DetectionManager : ModuleManager
{

    [SerializeField] VisibilityManager visibilityManager;

    public LayerMask unitLayer;

    private Coroutine updateLoop;

    [SerializeField] private int frameSkip;

    DetectionModule[] detectionModules;

    public override void InitModuleManager()
    {
        base.InitModuleManager();

        detectionModules = managedModules.Cast<DetectionModule>().ToArray();
    }

    public override void StartModuleManager()
    {
        base.StartModuleManager();

        updateLoop = StartCoroutine(UpdateLoop());
    }

    private void OnEnable()
    {
        //if (managerStarted)
        //{
        //    updateLoop = StartCoroutine(UpdateLoop());
        //}
    }

    private void OnDisable()
    {
        //StopAllCoroutines();
    }

    public override void UnregisterModule(int id)
    {
        //StopAllCoroutines();

        detectionModules = detectionModules.Where(val => val.InstanceId != id).ToArray();
        managedModules = managedModules.Where(val => val.InstanceId != id).ToArray();

        //updateLoop = StartCoroutine(UpdateLoop());
    }

    private IEnumerator UpdateLoop()
    {
        Debug.Log("Coroutine starts");

        List<Unit> tempList = new List<Unit>();
        int moduleIndex = 0;

        while (true)
        {
            if (detectionModules.Length == 0)
            {
                yield return null;
                continue;
            }

            if (moduleIndex >= detectionModules.Length)
            {
                moduleIndex = 0;
            }

            DetectionLoop(detectionModules[moduleIndex], tempList);

            Debug.Log("Unit done " + this.gameObject.name);

            moduleIndex++;

            // Spread module updates across frames
            yield return null;

            //foreach (DetectionModule detectionModule in detectionModules)
            //{
            //    DetectionLoop(detectionModule, tempList);

            //    //tempList = DetectEnemies(detectionModule.ObservPos.position, detectionModule);

            //    ////now we set the unitdata for the detected enemies to say tehy are detected by the team that detected them
            //    //foreach (Unit detected in tempList)
            //    //{
            //    //    visibilityManager.SetDetected(detected.InstanceId, detectionModule.DetectionTime, detectionModule.TeamId);
            //    //} 
                
            //    yield return null;

            //    Debug.Log("Unit done");
            //}

            ////this is so unity doesn't crash when there are zero units lmoa
            //yield return null;
        }
    }

    private void DetectionLoop(DetectionModule detectionModule, List<Unit> tempList)
    {
        tempList = DetectEnemies(detectionModule.ObservPos.position, detectionModule);

        //now we set the unitdata for the detected enemies to say tehy are detected by the team that detected them
        foreach (Unit detected in tempList)
        {
            visibilityManager.SetDetected(detected.InstanceId, detectionModule.DetectionTime, detectionModule.TeamId);
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
                if (unitCode.TeamId == detectionModule.TeamId | visibilityManager.IsTargetDetected(unitCode.InstanceId, detectionModule.TeamId, 1f))
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
            }
        }

        return detectedUnits;
    }
}
