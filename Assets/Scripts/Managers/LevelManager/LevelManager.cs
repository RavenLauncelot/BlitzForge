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

    private RealPlayer realPlayer;

    private UnitManager unitManager;
    [SerializeField] private WinConditions[] winConditions;

    public enum WinMode
    {
        winAllObjectives,
        winOneObjective
    }
    [SerializeField] private WinMode winMode;

    //this will spawn teams and unit managers
    public void Awake()
    {
        //spawning unitManager
        GameObject unitMan = Instantiate(unitManagerPrefab);
        unitManager = unitMan.GetComponent<UnitManager>();
        unitManager.InitManager(spawnData, this);        
       
        //Initialising the unitUpdater - This is for custom updates needs to be initilised once all units are spawned
        GetComponent<UnitUpdate>().InitUnitUpdate();
    }

    public void StartGame()
    {
        unitManager.StartGame();

        realPlayer = GameObject.Find("PlayerController(Clone)").GetComponent<RealPlayer>();    
    }

    public void Update()
    {
        if (CheckWin(winMode) != TeamInfo.TeamId.None)
        {
            //Winning game logic here
            //tell game manager
            GameManager.instance.GameFinished(CheckWin(winMode));
        }

        if (realPlayer == null)
        {
            return;
        }
        if (realPlayer.RemainingUnits() == 0)
        {
            GameManager.instance.GameFinished(TeamInfo.TeamId.None);
        }
    }

    public TeamInfo.TeamId CheckWin(LevelManager.WinMode winMode)
    {
        if (winMode == WinMode.winAllObjectives)
        {
            if (winConditions.All(val => val == winConditions[0]))
            {
                return winConditions[0].WinStates;
            }

            else
            {
                return TeamInfo.TeamId.None;
            }
        }

        else
        {
            foreach(WinConditions winCon in winConditions)
            {
                if (winCon.WinStates != TeamInfo.TeamId.None)
                {
                    return winCon.WinStates;
                }
            }

            return TeamInfo.TeamId.None;
        }        
    }
}
