using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameView : MonoBehaviour {
	[SerializeField]private Text[] completeTargetText;
	[SerializeField]private Text actionValueText;
	[SerializeField]private GameObject winUI;
	[SerializeField]private GameObject winUI_NextLevelObj;
	[SerializeField]private GameObject lossUI;
	[SerializeField]private GameObject cardList;
	[SerializeField]private Text useTimeText;
	[SerializeField]private UTweenColor actionValueEffect;
	[SerializeField]private Text changeOperationCountText;
	[SerializeField]private GameObject changeOperationCountObj;
	[SerializeField]private Image[] operationImages;

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

	public void ResetCompleteTargetEff(){
		for (int i = 0; i < completeTargetText.Length; i++) {
			completeTargetText [i].color = Color.black;
		}
	}

	public void CompleteTargetEff(int p_index){
		completeTargetText [p_index].color = Color.green;
	}

	public void SetActionValue(float p_value){
		actionValueText.text = string.Format("{0:F2}", p_value);
	}

	public void ActionValueFlash(bool p_active){
		if (p_active) {
			if (actionValueEffect.enabled == false)
				actionValueEffect.enabled = true;
		} 
		else {
			if (actionValueEffect.enabled == true) {
				actionValueEffect.enabled = false;
				actionValueText.color = Color.black;
			}
		}
	}

	public void ShowWinUI(float p_useTime, bool p_isGetStar){
		winUI.SetActive (true);
		if (Game.endlessMode == false && Game.NOWLEVEL != 499) {
			winUI_NextLevelObj.SetActive (true);
		} 
		else {
			winUI_NextLevelObj.SetActive (false);
		}

		useTimeText.text = I2.Loc.ScriptLocalization.Get("Game_UseTime") + p_useTime.ToString("#.##");
		if (p_isGetStar) {
			useTimeText.text += "獲得星星";
		}
		cardList.SetActive (false);
	}

	public void ShowLossUI(){
		lossUI.SetActive (true);
		cardList.SetActive (false);
	}

	public void ShowChangeOperationCountText(){
		changeOperationCountObj.SetActive (true);
	}

	public void HideChangeOperationCountText(){
		changeOperationCountObj.SetActive (false);
	}

	public void SetChangeOperationCountText(int p_count){
		changeOperationCountText.text = p_count.ToString ();
	}

	public void SetOperationBtnActive(string p_Operation){
		int _index = -1;
		if (p_Operation == "Addition") {
			_index = 0;
		}
		else if (p_Operation == "Subtraction") {
			_index = 1;
		}
		else if (p_Operation == "Multiplication") {
			_index = 2;
		}
		else if (p_Operation == "Division") {
			_index = 3;
		}


		for (int i = 0; i < operationImages.Length; i++) {
			if (i == _index) {
				operationImages [i].color = Color.green;
			} 
			else {
				operationImages [i].color = Color.white;
			}
		}
	}
}
