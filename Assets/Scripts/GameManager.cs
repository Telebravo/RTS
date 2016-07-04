using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum Team { Team1, Team2 }

public class GameManager : MonoBehaviour
{
    public static Team team;
    public static Unit selectedUnit;
    public static List<Unit> friendlyUnits;
    public static List<Unit> enemyUnits;
    public static List<Unit> visibleEnemies;
    public static CameraControlls controlls;

    [SerializeField]
    private Team _team = Team.Team1;

    void Awake()
    {
        team = _team;

        friendlyUnits = new List<Unit>();
        enemyUnits = new List<Unit>();
        visibleEnemies = new List<Unit>();
    }
}