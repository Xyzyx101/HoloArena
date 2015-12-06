using UnityEngine;
using System.Collections;

public class GameController : MonoBehaviour
{
    public Transform Player1Spawn;
    public Transform Player2Spawn;
    public RobotBuilder RobotBuilder1;
    public RobotBuilder RobotBuilder2;
    public ThirdPersonCamera Player1Camera;
    public ThirdPersonCamera Player2Camera;
    public PlayerController Player1;
    public PlayerController Player2;
    public GameState State;
    public CreationController Creator;
    public bool CursorLocked = false;

    // Use this for initialization
    void Start()
    {
        switch (State)
        {
            case (GameState.Arena):
                SetCursorLock(true);
                GameObject robot = RobotBuilder1.BuildRobot(Player1Spawn);
                robot.tag = "Player1";
                Player1 = robot.GetComponent<PlayerController>();
                Player1.CanMove = true;

                Player1Camera = GameObject.Find("Player1Camera").GetComponent<ThirdPersonCamera>();
                Player1Camera.Player = Player1.transform;
                Player1Camera.PlayerHead = Player1.HeadPosition;
                Player1Camera.PlayerLookAtTarget = Player1.LookAtTarget;

                GameObject Player1UIObj = GameObject.Find("Player1UI");
                UIController Player1UI = Player1UIObj.GetComponent<UIController>();
                Player1UI.Player = Player1;

                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SetCursorLock(!CursorLocked);
        }
    }

    void SetCursorLock(bool locked)
    {
        CursorLocked = locked;
        Cursor.visible = !CursorLocked;
        if(CursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        } 
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }
        
    }

    public Color GetTeam1PrimaryColor()
    {
        return Color.red;
    }

    public Color GetTeam1SecondaryColor()
    {
        return Color.grey;
    }

    public Color GetTeam2PrimaryColor()
    {
        return Color.blue;
    }

    Color GetTeam2SecondaryColor()
    {
        return Color.yellow;
    }

}

public enum GameState
{
    Player1Build,
    Player2Build,
    Arena
}