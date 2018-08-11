using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameSystem : MonoBehaviour {
	private enum OperationType{Addition, Subtraction, Multiplication, Division};
	private enum SystemStatue{Idle, Working};

	[Header("Data")]
	[SerializeField]private int[] completeTargets;
	[SerializeField]private OperationType operationState= OperationType.Addition;
	[SerializeField]private string[] initCardsID;
	[Header("====")]

	[SerializeField]private int maxCards = 9;
	[SerializeField]private List<Card> cards;
	private Queue<Card> cardsPool;
	[SerializeField]private Transform cardListTransform;
	[SerializeField]private Transform cardPoolTransform;
	[SerializeField]private GameView gameView;
	private SystemStatue systemStatue = SystemStatue.Idle;
	[SerializeField]private Card playerCard;


	void Start () {
		LoadLevelData();
		CreateCardPool();
		CreateCards();
		CreateCompleteTarget ();
	}

	private void CreateCompleteTarget(){
		completeTargets = new int[3];
		List<int> _range = new List<int> ();
		for (int i = 1; i < 100; i++) {
			_range.Add (i);
		}

		for (int i = 0; i < completeTargets.Length; i++) {

			int _index = UnityEngine.Random.Range (0, _range.Count);
			completeTargets [i] = _range[_index];
			_range.RemoveAt (_index);

		}

		gameView.InitCompleteTarget (completeTargets);
	}

	private void LoadLevelData(){
	}

	private void CreateCardPool(){;
		cardsPool = new Queue<Card>();

		for(int i=0; i< maxCards; i++){
			GameObject _obj = Instantiate(Resources.Load("Prefabs/Game/Card"))  as GameObject;
			Card _card = _obj.GetComponent<Card>();
			cardsPool.Enqueue(_card);
			_obj.transform.SetParent(cardPoolTransform, false);
		}
	}

	private void DestoryCard(int p_positionIndex){
		for(int i=0; i< cards.Count; i++){
			if(cards[i].GetPositionIndex() == p_positionIndex){
				cards[i].DestoryEff();
				cardsPool.Enqueue(cards[i]);
				cards[i].transform.SetParent(cardPoolTransform, false);
				cards.RemoveAt (i);
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
		cards.Add (_card);
		_card.Init(p_CardID, p_positionIndex, OnTouchCardEvent);
		_card.CreateEff();
		_card.transform.SetParent(cardListTransform, false);

		if (_card.GetCardType() == "Player") {
			playerCard = _card;
		}
	}

	private void CreateCards(){
		for(int i=0; i< 9; i++){
			CreateCard(initCardsID[i], i);
		}
	}

	void Update () {
		
	}

	public void BackToMenu(){
		Game.LoadScene ("MenuScene");
	}

	private void OnTouchCardEvent(int p_touchCardPositionIndex){
		systemStatue = SystemStatue.Working;

		if (IsNearby (playerCard.GetPositionIndex (), p_touchCardPositionIndex)) {
//			playerCard.
		}
		systemStatue = SystemStatue.Idle;
	}

	private bool IsNearby(int p_playerPositionIndex, int p_touchCardPositionIndex){
		bool _isNearby = false;

		if (p_touchCardPositionIndex == p_playerPositionIndex - 1 || //左邊
			p_touchCardPositionIndex == p_playerPositionIndex + 1 || //右邊
			p_touchCardPositionIndex == p_playerPositionIndex - 3 || //上面
			p_touchCardPositionIndex == p_playerPositionIndex + 3 //下面
		) {
			_isNearby = true;
		}

		return _isNearby;
	}

	IEnumerator IE_MoveCard(){
		yield return null;
	}

	public void OnChangeOperation(string p_Operation){
		operationState = (OperationType)Enum.Parse (typeof(OperationType), p_Operation);
	}
}
