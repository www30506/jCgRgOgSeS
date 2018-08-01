using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PageManerger_Example : MonoBehaviour {
	
	IEnumerator Start () {
		Debug.unityLogger.logEnabled = false;
		PageManerger.CloseAllPage ();
		yield return null;
		PageManerger.ChangePage (PageType.Test_MainPage);
	}
	
	void Update () {
		
	}
}
