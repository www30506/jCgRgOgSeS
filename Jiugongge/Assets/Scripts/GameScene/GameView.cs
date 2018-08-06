using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameView : MonoBehaviour {
	[SerializeField]private Text[] completeTargetText;

	void Start () {
		
	}
	
	void Update () {
		
	}

	public void InitCompleteTarget(int[] p_completeTarget){
		for (int i = 0; i < p_completeTarget.Length; i++) {
			completeTargetText [i].text = p_completeTarget [i].ToString ();
		}
	}
}
