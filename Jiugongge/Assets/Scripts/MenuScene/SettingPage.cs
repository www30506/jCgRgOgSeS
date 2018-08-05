using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingPage : Page_Base {

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
}
