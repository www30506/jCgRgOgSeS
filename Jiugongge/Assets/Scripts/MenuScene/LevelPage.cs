﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPage : Page_Base {

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

	public void ToGame(int p_level){
		print ("進入遊戲 關卡：" + p_level);
		Game.NOWLEVEL = p_level;
		Game.LoadScene ("GameScene");
	}
}
