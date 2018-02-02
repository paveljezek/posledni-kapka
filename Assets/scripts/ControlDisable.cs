using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlDisable : MonoBehaviour {


	PlayerController thePlayer;
	// Use this for initialization
	void Start () {
		thePlayer = FindObjectOfType<PlayerController>();
	}

	public void setPlayerControl(bool move) {
		thePlayer.setCanMove(move);
	}
	
	
}
