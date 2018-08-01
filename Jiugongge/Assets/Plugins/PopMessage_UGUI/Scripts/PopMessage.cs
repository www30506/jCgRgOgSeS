using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PopMessage : MonoBehaviour {
	public static PopMessage instance;
	[System.Serializable]
	public class PopMessageLoading{
		public GameObject Obj;
	}

	[System.Serializable]
	public class PopMessageConfirm{
		public GameObject Obj;
		public Text title;
		public Text description;
		public ConfirmCallBack confirmCallBack;
	}
	[System.Serializable]
	public class PopMessageYesNo{
		public GameObject Obj;
		public Text title;
		public Text description;
		public YesCallBack yesCallBack;
		public NoCallBack noCallBack;
	}

	[SerializeField]private PopMessageConfirm confirm;
	[SerializeField]private PopMessageYesNo yesNo;
	[SerializeField]private PopMessageLoading loading;

	void Start () {
		CheckEventSystemExist ();
	}

	void Update () {
	
	}

	public static void CreatePopMessage(){
		instance  = (Instantiate(Resources.Load("CPlugins/PopMessage/PopMessageCanvas")) as GameObject).GetComponent<PopMessage>();
	}

	/********************************************************
	 * 						Loading視窗
	 * ******************************************************/
	public static void OpenLoading(){
		if (instance == null) {
			CreatePopMessage ();
		}
		instance.M_OpenLoading ();
	}

	public void M_OpenLoading(){
		loading.Obj.SetActive (true);
	}

	public static void CloseLoading(){
		if (instance == null) {
			CreatePopMessage ();
		}
		instance.M_CloseLoading ();
	}

	public void M_CloseLoading(){
		loading.Obj.SetActive (false);
	}

	/********************************************************
	 * 						確認視窗
	 * ******************************************************/
	public delegate void ConfirmCallBack ();
	public static void ConfirmMessage(string p_Title, string p_Text){
		PopMessage.ConfirmMessage (p_Title, p_Text, null);
	}
	public static void ConfirmMessage(string p_Title, string p_Text, ConfirmCallBack p_confirmCallBack){
		if (instance == null) {
			CreatePopMessage ();
		}
		instance.M_ConfirmMessage (p_Title, p_Text, p_confirmCallBack);
	}

	public void M_ConfirmMessage (string p_Title, string p_Text, ConfirmCallBack p_confirmCallBack){
		ShowAnim(confirm.Obj);
		confirm.title.text = p_Title;
		confirm.description.text = p_Text; 

		confirm.confirmCallBack = p_confirmCallBack;
	}

	public void ClcikConfirm(){

		if (confirm.confirmCallBack != null) {
			confirm.confirmCallBack.Invoke ();
		}

		HideAnim(confirm.Obj);
	}

	/********************************************************
	 * 						YesNo視窗
	 * ******************************************************/
	public delegate void YesCallBack ();
	public delegate void NoCallBack ();

	public static void YesNoMessage(string p_Title, string p_descript, YesCallBack p_yesCallBack, NoCallBack p_noCallBack){
		if (instance == null) {
			CreatePopMessage ();
		}
		instance.M_YesNoMessage(p_Title, p_descript, p_yesCallBack, p_noCallBack);
	}

	public void M_YesNoMessage(string p_Title, string p_descript, YesCallBack p_yesCallBack, NoCallBack p_noCallBack){
		ShowAnim (yesNo.Obj);
		yesNo.title.text = p_Title;
		yesNo.description.text = p_descript;
		yesNo.yesCallBack = p_yesCallBack;
		yesNo.noCallBack = p_noCallBack;
	}

	public void ClickYes(){
		if (yesNo.yesCallBack != null) {
			yesNo.yesCallBack.Invoke ();
		} else {
			Debug.LogError ("Dont have functionName");
		}
		HideAnim (yesNo.Obj);
	}

	public void ClickNo(){
		if (yesNo.noCallBack != null) {
			yesNo.noCallBack.Invoke ();
		} else {
			Debug.LogError ("Dont have functionName");
		}
		HideAnim (yesNo.Obj);
	}

	/********************************************************
	 * 					共用
	 * *******************************************************/
	private void ShowAnim(GameObject p_obj){
		p_obj.SetActive (true);
	}

	private void HideAnim(GameObject p_obj){
		p_obj.SetActive (false);
	}

	private void CheckEventSystemExist(){
		if (!GameObject.Find ("EventSystem")) {
			Instantiate (Resources.Load ("Plugins/PopMessage/EventSystem"));
		}
	}
}
