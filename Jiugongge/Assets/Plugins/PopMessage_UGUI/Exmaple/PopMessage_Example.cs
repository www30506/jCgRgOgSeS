using UnityEngine;
using System.Collections;

public class PopMessage_Example : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.A)) {
			PopMessage.ConfirmMessage ("Title", "123");
		}
		if (Input.GetKeyDown (KeyCode.S)) {
			PopMessage.YesNoMessage("title", "5678", B, C);
		}

		if (Input.GetKeyDown (KeyCode.D)) {
			PopMessage.ConfirmMessage ("Title", "123", A);
		}

		if (Input.GetKeyDown (KeyCode.F)) {
			PopMessage.OpenLoading ();
		}

		if (Input.GetKeyDown (KeyCode.G)) {
			PopMessage.CloseLoading ();
		}
	}

	public void A(){
		print ("Click Confrim");
	}

	public void B(){
		print ("Click Yes");
	}
	public void C(){
		print ("Click No");
	}
}
