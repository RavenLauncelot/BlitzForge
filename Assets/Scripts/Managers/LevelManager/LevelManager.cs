using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using static UnitManager;
using UnityEngine.Rendering;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;

public class LevelManager : MonoBehaviour
{
    //This loads the entire level. Can set the amount of teams and certain Data 

    [SerializeReference]
    private List<SpawnData> spawnData;

    [SerializeField] private GameObject unitManagerPrefab;
    [SerializeField] private GameObject unitHandler;

    [SerializeField] private GameObject playerController;
    [SerializeField] private GameObject AiController;

    private Dictionary<int, Unit> idUnitDictionary;

    //this will spawn teams and unit managers
    public void Awake()
    {
        //spawning unitManager
        GameObject unitMan = Instantiate(unitManagerPrefab);
        UnitManager unitManager = unitMan.GetComponent<UnitManager>();
        unitManager.InitManager(spawnData, this);        
       
        //Initialising the unitUpdater - This is for custom updates needs to be initilised once all units are spawned
        GetComponent<UnitUpdate>().InitUnitUpdate();
    }
}
