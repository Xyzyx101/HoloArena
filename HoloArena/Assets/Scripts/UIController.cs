using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UIController : MonoBehaviour {

    public Image PowerMeterBar;
    public PlayerController Player;
	
	// Update is called once per frame
	void Update () {
        PowerMeterBar.rectTransform.localScale = new Vector3(Player.Power / Player.MaxPower, 1f, 1f);
	}
}
