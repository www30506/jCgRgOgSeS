using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameView : MonoBehaviour {
	[SerializeField]private Text[] completeTargetText;
	[SerializeField]private Text actionValueText;
	[SerializeField]private GameObject winUI;
	[SerializeField]private GameObject lossUI;
	[SerializeField]private GameObject cardList;
	[SerializeField]private Text useTimeText;
	[SerializeField]private UTweenColor actionValueEffect;

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
}
