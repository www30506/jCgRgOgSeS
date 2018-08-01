using UnityEngine;
using System.Collections;
using I2.Loc.SimpleJSON;

public class SimpleJSON_Example : MonoBehaviour {

	// Use this for initialization
	void Start () {
		var _json = JSON.Parse("{\"match_detail\":{\"legnum\":\"3\",\"legs\":[{\"order\":\"1\",\"game\":\"202\"},{\"order\":\"2\",\"game\":\"301\"}]}}");

		print(_json["match_detail"]["legnum"]);

		print(_json["match_detail"]["legs"][1]["game"]);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
