using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSystem : MonoBehaviour {
	[SerializeField]private int maxCards = 9;
	[SerializeField]private List<Card> cards;
	private Queue<Card> cardsPool;
	[SerializeField]private Transform cardListTransform;
	[SerializeField]private Transform cardPoolTransform;

	private enum SystemStatue{Idle, Working};
	private SystemStatue systemStatue = SystemStatue.Idle;

	void Start () {
		LoadLevelData();
		CreateCardPool();
		CreateCards();

//		for (int i = 0; i < PD.DATA ["CardTable"].Count; i++) {
//			print (PD.DATA ["CardTable"] [i.ToString ()] ["Type"]);
//		}
	}

	private void LoadLevelData(){
	}

	private void CreateCardPool(){
		cardsPool = new Queue<Card>();

		for(int i=0; i< maxCards; i++){
			GameObject _obj = Instantiate(Resources.Load("Prefabs/Game/Card"))  as GameObject;
			Card _card = _obj.GetComponent<Card>();
			cardsPool.Enqueue(_card);
			_obj.transform.SetParent(cardPoolTransform, false);
		}
	}

	private void DestoryCard(int p_positionIndex){
		for(int i=0; i< maxCards; i++){
			if(cards[i].GetPositionIndex() == p_positionIndex){
				cards[i].DestoryEff();
				cardsPool.Enqueue(cards[i]);
				cards[i].transform.SetParent(cardPoolTransform, false);
			}
		}
	}

	private Card GetCard(int p_positionIndex){
		for(int i=0; i< maxCards; i++){
			if(cards[i].GetPositionIndex() == p_positionIndex){
				return cards[i];
			}
		}
		Debug.LogError("錯誤 沒有取得卡片 位置編號 : " + p_positionIndex);
		return null;
	}

	private void CreateCard(string p_CardID, int p_positionIndex){
		Card _card = cardsPool.Dequeue();
		_card.Init(p_CardID, p_positionIndex);
		_card.CreateEff();
		_card.transform.SetParent(cardListTransform, false);
	}

	private void CreateCards(){
		for(int i=0; i< 9; i++){
			CreateCard((i+1).ToString(), i);
		}
	}

	void Update () {
		
	}

	public void BackToMenu(){
		Game.LoadScene ("MenuScene");
	}
}
