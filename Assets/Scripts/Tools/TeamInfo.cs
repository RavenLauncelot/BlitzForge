using UnityEngine;

public static class TeamInfo
{
    public enum TeamId
    {
        None,
        PlayerTeam,
        TeamA,
        TeamB,
        TeamC,
        TeamD,
        TeamE,
        TeamF,
    }

    public static Color[] TeamColors =
    {
        Color.green,
        Color.red,
        Color.yellow,
        Color.blue,
        Color.gray,
        Color.cyan,
        Color.magenta,
    };

    public static Color GetTeamColor(TeamId teamId)
    {
        if (teamId == TeamId.None)
        {
            return Color.white;
        }

        return TeamColors[(int)teamId-1];
    }
}
