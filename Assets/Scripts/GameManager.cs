using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Team { Team0, Team1, Team2 }

public class GameManager : MonoBehaviour
{
    public static Team team;
    public static int teams;
    public static Unit selectedUnit;
    public static List<Unit>[] units;
    public static List<Unit>[] visibleEnemies;
    public static CameraControlls controlls;

    [SerializeField]
    private Team _team = Team.Team1;

    void Awake()
    {
        team = _team;
        teams = System.Enum.GetNames(typeof(Team)).Length;

        units = new List<Unit>[teams];
        visibleEnemies = new List<Unit>[teams];
        for (int i = 0; i < teams; i++)
        {
            units[i] = new List<Unit>();
            visibleEnemies[i] = new List<Unit>();
        }

    }
}