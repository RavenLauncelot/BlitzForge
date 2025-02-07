using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Level/LevelUnitData")]
public class LevelUnitData : ScriptableObject
{
    public enum teamName
    {
        teamA,
        teamB,
        teamC,
        teamD,
        teamE,
        teamF,
        teamG,
        teamH,
    }

    public teamName team;

    public int infantry;
    public int heavyInfantry;

    public int battleTank;
    public int artilleryTank;

    public int infantryTransporter;

    public int debugInfantry;
}
