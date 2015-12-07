using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : Singleton<GameController>
{
    protected GameController() { }
    public Transform Player1Spawn;
    public Transform Player2Spawn;
    public RobotBuilder RobotBuilder1;
    public RobotBuilder RobotBuilder2;
    public ThirdPersonCamera Player1Camera;
    public ThirdPersonCamera Player2Camera;
    public PlayerController Player1;
    public PlayerController Player2;
    public GameState State;
    public bool CursorLocked = false;
    public ColorLookup MyColorLookup;
    public Color Team1PrimaryColor;
    public Color Team1SecondaryColor;
    public Color Team2PrimaryColor;
    public Color Team2SecondaryColor;

    void Awake()
    {
        if (FindObjectsOfType<GameController>().Length > 1)
        {
            DestroyImmediate(gameObject);
            return;
        }
        DontDestroyOnLoad(transform.gameObject);
        Team1PrimaryColor = MyColorLookup.GetColorData("Red").MyColor;
        Team1SecondaryColor = MyColorLookup.GetColorData("Silver").MyColor;
        Team2PrimaryColor = MyColorLookup.GetColorData("Blue").MyColor;
        Team2SecondaryColor = MyColorLookup.GetColorData("Gold").MyColor;
    }

    void OnEnable()
    {
        Debug.LogWarning("Game controller OnEnable");
    }

    void Start()
    {
        Debug.LogWarning("Game controller Start");
        switch (State)
        {
            case GameState.Player1Build:

                break;
            case GameState.Player2Build:

                break;
            case (GameState.Arena):
               

                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetCursorLock(!CursorLocked);
        }
    }

    void SetCursorLock(bool locked)
    {
        CursorLocked = locked;
        Cursor.visible = !CursorLocked;
        if (CursorLocked)
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
        }

    }

    public void InitArena()
    {
        Player1Spawn = GameObject.Find("Player1Spawn").transform;
        Player2Spawn = GameObject.Find("Player2Spawn").transform;

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
    }

    public void BeginGame()
    {
        if (State == GameState.Player1Build)
        {
            Debug.LogWarning("Load build 2");
            State = GameState.Player2Build;
            Application.LoadLevel("CharacterCreator");
        }
        else if (State == GameState.Player2Build)
        {
            Debug.LogWarning("Load Arena");
            State = GameState.Arena;
            Application.LoadLevel("Arena");
        }
    }

    public void SetPrimaryColor(Object uiObject)
    {
        Dropdown colorDropdown = ((GameObject)uiObject).GetComponent<Dropdown>();
        if (State == GameState.Player1Build)
        {
            Team1PrimaryColor = MyColorLookup.GetColorData(colorDropdown.options[colorDropdown.value].text).MyColor;
        }
        else
        {
            Team2PrimaryColor = MyColorLookup.GetColorData(colorDropdown.options[colorDropdown.value].text).MyColor;
        }
    }

    public void SetSecondaryColor(Object uiObject)
    {
        Dropdown colorDropdown = ((GameObject)uiObject).GetComponent<Dropdown>();
        if (State == GameState.Player1Build)
        {
            Team1SecondaryColor = MyColorLookup.GetColorData(colorDropdown.options[colorDropdown.value].text).MyColor;
        }
        else
        {
            Team2SecondaryColor = MyColorLookup.GetColorData(colorDropdown.options[colorDropdown.value].text).MyColor; ;
        }
    }

    public Color GetTeam1PrimaryColor()
    {
        return Team1PrimaryColor;
    }

    public Color GetTeam1SecondaryColor()
    {
        return Team1SecondaryColor;
    }

    public Color GetTeam2PrimaryColor()
    {
        return Team2PrimaryColor;
    }

    Color GetTeam2SecondaryColor()
    {
        return Team2SecondaryColor;
    }

    public RobotBuilder GetBuilder()
    {
        if (State == GameState.Player1Build)
        {
            return RobotBuilder1;
        }
        else
        {
            return RobotBuilder2;
        }
    }

}

public enum GameState
{
    Player1Build,
    Player2Build,
    Arena
}