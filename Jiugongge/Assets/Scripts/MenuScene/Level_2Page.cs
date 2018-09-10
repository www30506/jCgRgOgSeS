using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[ExecuteInEditMode]
public class Level_2Page : Page_Base {
	public Transform aa;

	void Start () {
	}

	void Update () {
//		Text[] _ts = aa.GetComponentsInChildren<Text> ();
//		for (int i = 0; i < _ts.Length; i++) {
//			_ts[i].transform.parent.gameObject.name = i.ToString();
//			_ts [i].text = (i+1).ToString ();
//		}
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
		Game.NOWLEVEL = p_level + (Game.CLASS * 50);
		Game.LoadScene ("GameScene");
	}
}
