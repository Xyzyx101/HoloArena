using UnityEngine;
using System.Collections;

public class ArenaInitializer : MonoBehaviour
{
    void Start()
    {
        GameController myGameController = FindObjectOfType<GameController>();
        myGameController.InitArena();
    }
}
