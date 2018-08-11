using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour {
	[SerializeField]private SpriteRenderer m_SpriteRenderer;
	[SerializeField]private int positionIndex;
	[SerializeField]private string cardID;
	[SerializeField]private int m_value;
	[SerializeField]private string m_type;
	[SerializeField]private string m_name;
	[SerializeField]private TextMesh m_valueTextMesh;

	private Transform thisTransform;

	void Awake(){
		thisTransform = this.transform;
	}

	void Start () {
		
	}
	
	void Update () {
		
	}

	public void OnMouseUp(){
		print("【卡片放開】 cardID : " + cardID + "  , PositionIndex : " + positionIndex);
	}

	public void OnMouseDown(){
		print("【卡片按下】 cardID : " + cardID + "  , PositionIndex : " + positionIndex);
	}

	public void Init(string p_cardID,int p_positionIndex){
		positionIndex = p_positionIndex;
		cardID = p_cardID;
		m_value = int.Parse(PD.DATA ["CardTable"] [p_cardID] ["Value"].ToString());
		m_type = PD.DATA ["CardTable"] [p_cardID] ["Type"].ToString();
		m_name = PD.DATA ["CardTable"] [p_cardID] ["Name"].ToString();

		//改變圖片現在沒用到
		//ChangeSprite(p_cardID);
		SetValue();
		SetPosition(p_positionIndex);
		this.gameObject.name = p_cardID;
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
		}

		if (m_type == "Null") {
			m_valueTextMesh.text = "";
		}
		else {
			m_valueTextMesh.text = m_value.ToString ();
		}
	}

	private void SetPosition(int p_cardIndex){
		Vector3 _newPosition = Vector3.zero;
		switch(p_cardIndex){
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

	public int GetPositionIndex(){
		return positionIndex;
	}

	public void SetPositionIndex(int p_positionIndex){
		positionIndex = p_positionIndex;
	}

	public void DestoryEff(){
		print("【卡片消滅特效】 cardID : " + cardID + "  , PositionIndex : " + positionIndex);
	}

	public void CreateEff(){
		print("【卡片生成特效】 cardID : " + cardID + "  , PositionIndex : " + positionIndex);
	}
}