using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameView_Poker : MonoBehaviour {
	[SerializeField]private GameObject winUI;
	[SerializeField]private GameObject cardList;
	[SerializeField]private Text score;
	void Start () {

	}

	void Update () {

	}

	public void SetScore(int p_Score){
		score.text = p_Score.ToString ();
	}

	public void ShowWinUI(float p_useTime, bool p_isGetStar){
		winUI.SetActive (true);

		cardList.SetActive (false);
	}
}
