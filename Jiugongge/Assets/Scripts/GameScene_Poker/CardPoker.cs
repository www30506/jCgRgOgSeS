using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPoker: MonoBehaviour {
	[SerializeField]private SpriteRenderer m_SpriteRenderer;
	[SerializeField]private int positionIndex;
	[SerializeField]private string cardID;
	[SerializeField]private string m_value;
	[SerializeField]private string m_type;
	[SerializeField]private string m_name;
	[SerializeField]private TextMesh m_valueTextMesh;

	private Transform thisTransform;
	public delegate void TouchCardHandler(int p_touchCardPositionIndex);
	private event TouchCardHandler TouchCardEvent;
	private UTweenScale destoryEff;

	void Awake(){
		thisTransform = this.transform;
		destoryEff = this.GetComponent<UTweenScale> ();
	}

	void Start () {
		
	}
	
	void Update () {
		
	}

	public string GetCardID(){
		return cardID;
	}

	public void OnMouseUp(){
		print("【點擊卡片】名稱 ： " + m_name +"  , cardID : " + cardID + "  , PositionIndex : " + positionIndex);
		TouchCardEvent.Invoke (positionIndex);
	}


	public void OnMouseDown(){
		//print("【卡片按下】 cardID : " + cardID + "  , PositionIndex : " + positionIndex);
	}

	public void Init(string p_cardID,int p_positionIndex, TouchCardHandler p_onTouchCardEvent){
		cardID = p_cardID;
		m_value = PD.DATA ["CardTable"] [p_cardID] ["Value"].ToString();
		m_type = PD.DATA ["CardTable"] [p_cardID] ["Type"].ToString();
		m_name = PD.DATA ["CardTable"] [p_cardID] ["Name"].ToString();

		//改變圖片現在沒用到
		//ChangeSprite(p_cardID);
		SetValue();
		SetPosition(p_positionIndex);
		#if UNITY_EDITOR
		this.gameObject.name = m_name;
		#endif

		TouchCardEvent += p_onTouchCardEvent;
	}

	public void  ResetCard(string p_cardID){
		cardID = p_cardID;
		m_value = PD.DATA ["CardTable"] [p_cardID] ["Value"].ToString();
		m_type = PD.DATA ["CardTable"] [p_cardID] ["Type"].ToString();
		m_name = PD.DATA ["CardTable"] [p_cardID] ["Name"].ToString();

		//改變圖片現在沒用到
		//ChangeSprite(p_cardID);
		SetValue();
		#if UNITY_EDITOR
		this.gameObject.name = m_name;
		#endif
	}

	public void ResetPositionIndex(int p_positionIndex){
		SetPosition(p_positionIndex);
	}

	private void ChangeSprite(string p_cardID){
		m_SpriteRenderer.sprite = Resources.Load<Sprite> ("Textures/Cards/" + p_cardID);
	}

	private void SetValue(){
		switch (m_type) {
		case "Base":
			m_valueTextMesh.color = Color.black;
			break;
		case "Action":
			m_valueTextMesh.color = Color.green;
			break;
		case "Null":
			m_valueTextMesh.color = Color.black;
			break;
		case "Player":
			m_valueTextMesh.color = Color.red;
			break;
		case "Question":
			m_valueTextMesh.color = Color.red;
			break;
		case "Marvel":
			m_valueTextMesh.color = Color.blue;
			break;
		case "ChangeOperation":
			m_valueTextMesh.color = Color.yellow;
			break;
		}

		if (m_type == "Null") {
			m_valueTextMesh.text = "";
		}
		else if(m_type == "Question"){
			m_valueTextMesh.text = "?";
		}
		else if(m_type == "Marvel"){
			m_valueTextMesh.text = "!";
		}
		else {
			m_valueTextMesh.text = m_value;
		}
	}

	public void SetPosition(int p_positionIndex){
		positionIndex = p_positionIndex;

		Vector3 _newPosition = Vector3.zero;
		switch(p_positionIndex){
		case 0:
			_newPosition = new Vector3(-2,2,0);
			break;
		case 1:
			_newPosition = new Vector3(0,2,0);
			break;
		case 2:
			_newPosition = new Vector3(2,2,0);
			break;
		case 3:
			_newPosition = new Vector3(-2,0,0);
			break;
		case 4:
			_newPosition = new Vector3(0,0,0);
			break;
		case 5:
			_newPosition = new Vector3(2,0,0);
			break;
		case 6:
			_newPosition = new Vector3(-2,-2,0);
			break;
		case 7:
			_newPosition = new Vector3(0,-2,0);
			break;
		case 8:
			_newPosition = new Vector3(2,-2,0);
			break;
		}
		thisTransform.localPosition = _newPosition;
	}

	public IEnumerator IE_SetPosition(int p_positionIndex){
		positionIndex = p_positionIndex;

		Vector3 _newPosition = Vector3.zero;
		switch(p_positionIndex){
		case 0:
			_newPosition = new Vector3(-2,2,0);
			break;
		case 1:
			_newPosition = new Vector3(0,2,0);
			break;
		case 2:
			_newPosition = new Vector3(2,2,0);
			break;
		case 3:
			_newPosition = new Vector3(-2,0,0);
			break;
		case 4:
			_newPosition = new Vector3(0,0,0);
			break;
		case 5:
			_newPosition = new Vector3(2,0,0);
			break;
		case 6:
			_newPosition = new Vector3(-2,-2,0);
			break;
		case 7:
			_newPosition = new Vector3(0,-2,0);
			break;
		case 8:
			_newPosition = new Vector3(2,-2,0);
			break;
		}

		Vector3 _prePosition = thisTransform.localPosition;
		Vector3 _distance = _newPosition - _prePosition;
		float _tempTime = 0;

		while (_tempTime < 0.3f) {
			thisTransform.localPosition = _prePosition + (_distance * _tempTime / 0.3f);
			_tempTime += Time.deltaTime;
			yield return null;
		}

		thisTransform.localPosition = _newPosition;
	}

	public int GetPositionIndex(){
		return positionIndex;
	}

	public string GetCardType(){
		return m_type;
	}

	public string GetCardName(){
		return m_name;
	}

	public IEnumerator Destory(){
		print("【卡片消滅特效】 cardID : " + cardID + "  , PositionIndex : " + positionIndex);
		TouchCardEvent = null;
		yield return StartCoroutine (IE_DestoryEff());
	}

	IEnumerator IE_DestoryEff(){
		destoryEff.enabled = true;
		destoryEff.ReSetToStart ();
		yield return new WaitForSeconds (destoryEff.Duration);
	}


	public IEnumerator CreateEff(){
		print("【卡片生成特效】 cardID : " + cardID + "  , PositionIndex : " + positionIndex);
		this.transform.localScale = Vector3.one;
		yield return null;
	}

	public string GetCardValue(){
		return m_value;
	}
}