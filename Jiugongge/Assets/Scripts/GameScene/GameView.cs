﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameView : MonoBehaviour {
	[SerializeField]private Text[] completeTargetText;
	[SerializeField]private Text actionValueText;
	[SerializeField]private GameObject winUI;
	[SerializeField]private GameObject lossUI;
	[SerializeField]private GameObject cardList;

	void Start () {
		
	}
	
	void Update () {
		
	}

	public void InitCompleteTarget(int[] p_completeTarget){
		for (int i = 0; i < p_completeTarget.Length; i++) {
			completeTargetText [i].text = p_completeTarget [i].ToString ();
		}

		for (int i = p_completeTarget.Length; i < 5; i++) {
			completeTargetText [i].transform.parent.gameObject.SetActive (false);
		}
	}

	public void CompleteTargetEff(int p_index){
		completeTargetText [p_index].color = Color.green;
	}

	public void SetActionValue(float p_value){
		actionValueText.text = p_value.ToString ();
	}

	public void ShowWinUI(){
		winUI.SetActive (true);
		cardList.SetActive (false);
	}

	public void ShowLossUI(){
		lossUI.SetActive (true);
		cardList.SetActive (false);
	}
}
