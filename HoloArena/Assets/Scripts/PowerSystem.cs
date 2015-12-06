using UnityEngine;
using System.Collections;

public class PowerSystem : MonoBehaviour
{
    public PowerPod[] PowerPods;
    public GameController MyGameController;
    public float PowerGainInterval;
    private float PowerGainTimer;

    public PlayerController Player1;
    public PlayerController Player2;

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < PowerPods.Length; ++i)
        {
            PowerPods[i].Team1Color = MyGameController.GetTeam1PrimaryColor();
            PowerPods[i].Team2Color = MyGameController.GetTeam2PrimaryColor();
        }
        PowerGainTimer = PowerGainInterval;
        Player1 = MyGameController.Player1;
        Player2 = MyGameController.Player2;
    }

    // Update is called once per frame
    void Update()
    {
        PowerGainTimer -= Time.deltaTime;
        if (PowerGainTimer < 0)
        {
            PowerGainTimer = PowerGainInterval;

            // Minimal Power Gain
            float player1Power = 0.1f;
            float player2Power = 0.1f;
            for (int i = 0; i < PowerPods.Length; ++i)
            {
                if (PowerPods[i].CapturePercent < PowerPods[i].CaptureMax)
                {
                    continue;
                }
                if (PowerPods[i].Owner == 1)
                {
                    player1Power += PowerPods[i].PowerAmount;
                }
                else if (PowerPods[i].Owner == 2)
                {
                    player2Power += PowerPods[i].PowerAmount;
                }
            }
            if (Player1 != null)
            {
                Player1.Power = Mathf.Min(Player1.MaxPower, Player1.Power + player1Power);
            }
            if (Player2 != null)
            {
                Player2.Power = Mathf.Min(Player2.MaxPower, Player2.Power + player2Power);
            }
        }
    }
}
