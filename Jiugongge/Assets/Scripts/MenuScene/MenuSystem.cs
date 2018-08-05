using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuSystem : MonoBehaviour {
	
	IEnumerator Start () {
		PageManerger.CloseAllPage ();
		yield return null;
		PageManerger.ChangePage (PageType.MainPage);
	}
	
	void Update () {
		
	}

}
