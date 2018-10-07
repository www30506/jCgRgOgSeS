using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPage : Page_Base {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	protected override void OnOpen (){

		print ("Test_MainPage Open");
	}

	protected override IEnumerator IE_OnOpen ()
	{
		print ("Test_MainPage IE_Open");
		yield return null;
	}

	protected override void OnClose (){

		print ("Test_MainPage Close");
	}

	protected override IEnumerator IE_OnClose ()
	{
		print ("Test_MainPage IE_Close");
		yield return null;
	}

	public void ToSettingPage(){
		print ("設定頁面");
		PageManerger.ChangePage (PageType.SettingPage);
	}

	public void ToPokerMode(){
		print ("撲克牌模式");
		Game.endlessMode = false;
		Game.LoadScene ("GameScene_Poker");
	}

	public void ToOperationMode(){
		print ("無盡模式");
		Game.endlessMode = true;
		Game.LoadScene ("GameScene");
	}
}
