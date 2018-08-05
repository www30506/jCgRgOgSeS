using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParseDataExample : MonoBehaviour {

	void Start () {
		print("====  讀取資料 ====");

		foreach(string key in PD.DATA ["Example"].Keys){
			print(PD.DATA["Example"] [key] ["name"]);
		}
		print(PD.DATA["Example"]["1"]["des"]);


		print("====  寫入資料 ====");
		print(PD.DATA ["Example"] ["1"] ["name"]);
		PD.DATA ["Example"] ["1"] ["name"] = "克羅";
		PD.DATA ["Example"] ["2"] ["name"] = "希司";
		PD.DATA ["Example"] ["1"] ["price"] = "10";
		PD.DATA ["Example"] ["2"] ["price"] = "20";
		print(PD.DATA ["Example"] ["1"] ["name"]);
		PD.Save(PD.DATA ["Example"], "Example");
	}

	void Update () {
		
	}
}
