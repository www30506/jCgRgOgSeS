using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelPage : Page_Base {
	[SerializeField]private Text[] levelNameTexts;


	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	protected override void OnOpen (){

		print ("Test_MainPage Open");
	}

	protected override IEnumerator IE_OnOpen (){
		print ("Test_MainPage IE_Open");
		SetLevelName ();
		yield return null;
	}

	private void SetLevelName(){
		for (int i = 0; i < levelNameTexts.Length; i++) {
			levelNameTexts [i].text = I2.Loc.ScriptLocalization.Get ("Menu_Level") + " " + (i + 1);
		}
	}

	protected override void OnClose (){

		print ("Test_MainPage Close");
	}

	protected override IEnumerator IE_OnClose ()
	{
		print ("Test_MainPage IE_Close");
		yield return null;
	}

	public void ToSelectLevel(int p_level){
		print ("選擇關卡：" + p_level);
		PageManerger.ChangePage (PageType.Level_2Page);
		Game.CLASS = p_level;
	}
}
