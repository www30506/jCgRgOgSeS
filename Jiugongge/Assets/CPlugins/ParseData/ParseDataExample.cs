using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParseDataExample : MonoBehaviour {

	void Start () {
		print("====  讀取資料 ====");
		//數量記得要-2 因為2、3行不應該在資料內
		for(int i=0;i<PD.DATA ["Example"].Count-2; i++){
			print(PD.DATA["Example"] [(i+1).ToString()] ["name"]);
		}

		print("====  寫入資料 ====");
		print(PD.DATA ["Example"] ["1"] ["name"]);
		PD.DATA ["Example"] ["1"] ["name"] = "克羅";
		PD.DATA ["Example"] ["2"] ["name"] = "希司";
		PD.DATA ["Example"] ["1"] ["price"] = "99";
		PD.DATA ["Example"] ["2"] ["price"] = "999";
		print(PD.DATA ["Example"] ["1"] ["name"]);
		PD.Save(PD.DATA ["Example"], "Example");

	}

	void Update () {
		
	}
}
