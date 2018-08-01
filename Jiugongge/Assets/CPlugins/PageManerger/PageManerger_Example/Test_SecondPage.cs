using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_SecondPage : Page_Base {

	void Start () {

	}

	void Update () {

	}

	protected override void OnOpen (){

		print ("Test_SecondPage Open");
	}

	protected override IEnumerator IE_OnOpen ()
	{
		print ("Test_SecondPage IE_Open");
		yield return null;
	}

	protected override void OnClose (){

		print ("Test_SecondPage Close");
	}

	protected override IEnumerator IE_OnClose ()
	{
		print ("Test_SecondPage IE_Close");
		yield return null;

	}


	public void OnToMainPage(){
		PageManerger.ChangePage (PageType.Test_MainPage);
	}
}
