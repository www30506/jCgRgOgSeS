using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PageType{Test_MainPage, Test_SecondPage}
public class PageManerger : MonoBehaviour {
	
	[Header("開啟和關閉頁面是否同時執行")]
	[SerializeField]private bool isAnimSynchronize = true;
	public static PageManerger instance;
	private List<Page_Base> pageList = new List<Page_Base>(); 
	[SerializeField]private List<PageType> pageTypeList = new List<PageType> ();
	private Transform canvas;
	private Page_Base[] allPages;
	private bool isWork = false;

	void Awake(){
		canvas = GameObject.Find ("Pages").transform;
		instance = this;
		allPages = canvas.GetComponentsInChildren<Page_Base> ();
	}

	void Start () {
	}
	
	void Update () {
		
	}
		
	/// <summary>
	/// 切換到目標頁面
	/// </summary>
	public static void ChangePage(PageType p_type){
		PageManerger.instance.M_ChangePage (p_type);
	}

	public void M_ChangePage(PageType p_type){
		if (isWork) {
			#if Clog
			print ("換頁中 指令無效");
			#endif
			return;
		}

		if(pageTypeList.Count > 0 && pageTypeList[pageTypeList.Count-1].ToString() == p_type.ToString()){
			return;
		}

		StartCoroutine (IE_ChangePage (p_type));
	}

	IEnumerator IE_ChangePage(PageType p_type){
		isWork = true;

		if (pageList.Count > 0) {
			if (isAnimSynchronize) {
				StartCoroutine (IE_CloseNowPage ());
			} 
			else {
				yield return StartCoroutine (IE_CloseNowPage ());
			}
		}

		for (int i = 0; i < allPages.Length; i++) {
			if (allPages [i].gameObject.name == p_type.ToString ()) {
				pageList.Add (allPages [i]);
			}
		}
		pageTypeList.Add (p_type);

		yield return StartCoroutine(OpenTargetPage (p_type));

		isWork = false;
	}

	IEnumerator OpenTargetPage(PageType p_type){
		for (int i = 0; i < allPages.Length; i++) {
			if (allPages [i].gameObject.name == p_type.ToString ()) {
				if(allPages [i].SwitchType == PageSwitchType.Active){
					allPages [i].gameObject.SetActive(true);
				}
				else if(allPages [i].SwitchType == PageSwitchType.Camera){
//					allPages [i].contentTransform.anchoredPosition = new Vector2(0,0);
				}
				else if(allPages [i].SwitchType == PageSwitchType.Canvas){
					allPages [i].canvas.enabled = true;
				}

				allPages [i].Open ();

				while (allPages [i].isOpening) {
					yield return null;
				}

				break;
			}
		}
	}

	IEnumerator IE_CloseNowPage(){
		for (int i = 0; i < allPages.Length; i++) {
			if (allPages [i].gameObject.name == pageTypeList[pageTypeList.Count-1].ToString()) {
				allPages [i].Close ();

				while (allPages [i].isClosing) {
					yield return null;
				}

				if(allPages [i].SwitchType == PageSwitchType.Active){
					allPages [i].gameObject.SetActive(false);
				}
				else if(allPages [i].SwitchType == PageSwitchType.Camera){
//					allPages [i].contentTransform.anchoredPosition = new Vector2(0,0);
				}
				else if(allPages [i].SwitchType == PageSwitchType.Canvas){
					allPages [i].canvas.enabled = false;
				}

				break;
			}
		}
	}

	/// <summary>
	/// 關閉所有頁面
	/// 初始化使用 其他時候會產生錯誤
	/// </summary>
	public static void CloseAllPage(){
		PageManerger.instance.M_CloseAllPage ();
	}

	public void M_CloseAllPage(){
		for (int i = 0; i < allPages.Length; i++) {
			allPages [i].Close();

			if(allPages [i].SwitchType == PageSwitchType.Active){
				allPages [i].gameObject.SetActive(false);
			}
			else if(allPages [i].SwitchType == PageSwitchType.Camera){
//				allPages [i].contentTransform.anchoredPosition = new Vector2(300,0);
			}
			else if(allPages [i].SwitchType == PageSwitchType.Canvas){
				allPages [i].canvas.enabled = false;
			}
		}
	}

	/// <summary>
	/// 返回上一頁
	/// </summary>
	public static void BackPage(){
		PageManerger.instance.M_BackPage ();
	}

	public void M_BackPage(){
		if (isWork) {
			#if Clog
			print ("換頁中 指令無效");
			#endif
			return;
		}
		StartCoroutine (IE_BackPage ());
	}

	IEnumerator IE_BackPage(){
		isWork = true;

		if (isAnimSynchronize) {
			StartCoroutine (IE_CloseNowPage ());
		}
		else {
			yield return StartCoroutine (IE_CloseNowPage ());
		}

		pageList.RemoveAt (pageList.Count - 1);
		pageTypeList.RemoveAt (pageTypeList.Count - 1);

		if (pageTypeList.Count > 0) {
			yield return StartCoroutine(OpenTargetPage (pageTypeList [pageTypeList.Count - 1]));
		}

		isWork = false;
	}

	/// <summary>
	/// 開啟頁面 不把現在的頁面關閉
	/// </summary>
	public static void OpenPage(PageType p_type){
		PageManerger.instance.M_OpenPage (p_type);
	}

	public void M_OpenPage(PageType p_type){
		if (isWork) {
			#if Clog
			print ("換頁中 指令無效");
			#endif
			return;
		}

		if(pageTypeList.Count > 0 && pageTypeList[pageTypeList.Count-1].ToString() == p_type.ToString()){
			return;
		}

		StartCoroutine(IE_OpenPage(p_type));
	}

	IEnumerator IE_OpenPage(PageType p_type){
		isWork = true;

		for (int i = 0; i < allPages.Length; i++) {
			if (allPages [i].gameObject.name == p_type.ToString ()) {
				pageList.Add (allPages [i]);
			}
		}
		pageTypeList.Add (p_type);

		yield return StartCoroutine(OpenTargetPage (p_type));

		isWork = false;
	}
}
