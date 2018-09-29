using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitSystem : MonoBehaviour {

	void Start () {
		PlayerData _playerData = PlayerData.Create ();
		_playerData.Save ();
		Game.LoadScene ("MenuScene");
	}
	
	void Update () {
		
	}

}
