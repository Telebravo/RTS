using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static CameraControlls controlls;
    public static Unit selectedUnit;
    public static string team;
    [SerializeField]
    private string _team = "Team 1";

    void Awake()
    {
        team = _team;
    }
    void OnTriggerExit(Collider other)
    {
        Destroy(other.gameObject);
    }

}
