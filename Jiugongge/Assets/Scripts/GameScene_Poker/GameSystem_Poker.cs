using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text.RegularExpressions;

public class GameSystem_Poker : MonoBehaviour {
	private enum SystemStatue{Idle, Working, Win , Loss};
	private enum CardMoveType{Left, Right, Up, Down};
	[SerializeField]private CardMoveType cardMoveState = CardMoveType.Left;

	[Header("Data")]
	[SerializeField]private Queue<int> drawCardsQueue;
	[SerializeField]private int score;
	[Header("====")]

	[SerializeField]private JudgingCardType judgingCardType;
	[SerializeField]private int drawCardIndex;

	[SerializeField]private int maxCards = 9;
	[SerializeField]private List<CardPoker> cards;
	private Queue<CardPoker> cardsPool;
	[SerializeField]private Transform cardListTransform;
	[SerializeField]private Transform cardPoolTransform;
	[SerializeField]private GameView_Poker gameView;
	private SystemStatue systemStatue = SystemStatue.Idle;
	[SerializeField]private CardPoker playerCard;
	private int prePlayerPositionIndex;

	void Start () {
		InitDrawCardList ();
		CreateCardPool ();
		CreateCards ();
		gameView.SetScore (0);
	}

	private void InitDrawCardList(){
		drawCardsQueue = new Queue<int>();
		List<int> _tempList = new List<int>();
		for (int i = 0; i < 52; i++) {
			_tempList.Add (i + 17);
		}

		while(_tempList.Count >0){
			int _random = UnityEngine.Random.Range (0, _tempList.Count);
			drawCardsQueue.Enqueue (_tempList [_random]);
			_tempList.RemoveAt (_random);
		}
	}

	private int GetTargetCount(int p_level){
		int _targetCount = p_level % 5;
		if (_targetCount == 0) {
			_targetCount = 5;
		}

		return _targetCount;
	}

	private void CreateCardPool(){;
		cardsPool = new Queue<CardPoker>();

		for(int i=0; i< maxCards; i++){
			GameObject _obj = Instantiate(Resources.Load("Prefabs/Game/CardPoker"))  as GameObject;
			CardPoker _card = _obj.GetComponent<CardPoker>();
			cardsPool.Enqueue(_card);
			_obj.transform.SetParent(cardPoolTransform, false);
		}
	}

	private CardPoker GetCard(int p_positionIndex){
		for(int i=0; i< maxCards; i++){
			if(cards[i].GetPositionIndex() == p_positionIndex){
				return cards[i];
			}
		}

		return null;
	}

	IEnumerator CreateCard(string p_CardID, int p_positionIndex){
		CardPoker _card = cardsPool.Dequeue();
		cards.Add (_card);
		_card.Init(p_CardID, p_positionIndex, OnTouchCardEvent);
		yield return StartCoroutine(_card.CreateEff());
		_card.transform.SetParent(cardListTransform, false);

		if (_card.GetCardType() == "Player") {
			playerCard = _card;
		}
	}

	private void CreateCards(){
		for(int i=0; i< 9; i++){
			if (i != 4) {
				StartCoroutine (CreateCard (drawCardsQueue.Dequeue ().ToString (), i));
			} 
			else {
				StartCoroutine (CreateCard (13.ToString(), i));
			}
		}
	}

	void Update () {
		if (Input.GetKeyUp (KeyCode.A)) {
			for (int i = 0; i < drawCardsQueue.Count; i++) {
				int _aa =drawCardsQueue.Dequeue();
				drawCardsQueue.Enqueue (_aa);
				print (_aa);
			}
		}
	}

	public void BackToMenu(){
		Game.LoadScene ("MenuScene");
	}

	private void OnTouchCardEvent(int p_touchCardPositionIndex){
		if (systemStatue != SystemStatue.Idle){
			print ("工作中");
			return;
		}

		StartCoroutine (IE_OnTouchCardEvent (p_touchCardPositionIndex));
	}

	IEnumerator IE_OnTouchCardEvent(int p_touchCardPositionIndex){
		systemStatue = SystemStatue.Working;

		if (IsNearby (playerCard.GetPositionIndex (), p_touchCardPositionIndex)) {
			CardPoker _card = GetCard (p_touchCardPositionIndex);

			DoCardAction (_card);

			yield return StartCoroutine(IE_DestoryCard(p_touchCardPositionIndex));

			yield return StartCoroutine (IE_MoveCard (p_touchCardPositionIndex));

			string[] _skillCards = new string[]{"14", "15", "16"};
			drawCardIndex = UnityEngine.Random.Range (0, _skillCards.Length);
			yield return StartCoroutine (CreateCard (_skillCards [drawCardIndex], GetCreateCardPositionIndex ()));
		}

		systemStatue = SystemStatue.Idle;
		yield return null;
	}
		
	private void SaveData(){
		PlayerData _playerData = PlayerData.Create ();

		_playerData.Save();
	}

	private int GetCreateCardPositionIndex(){
		int _CreateCardPositionIndex = 0;

		switch (cardMoveState) {
		case CardMoveType.Down:
			if (prePlayerPositionIndex - 3 >= 0) {
				_CreateCardPositionIndex = prePlayerPositionIndex -3;
			} 
			else {
				_CreateCardPositionIndex = prePlayerPositionIndex;
			}
			break;

		case CardMoveType.Up:
			if (prePlayerPositionIndex + 3 > 8) {
				_CreateCardPositionIndex = prePlayerPositionIndex;
			} 
			else {
				_CreateCardPositionIndex = prePlayerPositionIndex + 3;
			}
			break;

		case CardMoveType.Left:
			if (prePlayerPositionIndex % 3 == 2) {
				_CreateCardPositionIndex = prePlayerPositionIndex;
			} 
			else {
				_CreateCardPositionIndex = prePlayerPositionIndex + 1;
			}
			break;

		case CardMoveType.Right:
			if (prePlayerPositionIndex % 3 == 0) {
				_CreateCardPositionIndex = prePlayerPositionIndex;
			} 
			else {
				_CreateCardPositionIndex = prePlayerPositionIndex - 1;
			}
			break;
		}

		return _CreateCardPositionIndex;
	}

	IEnumerator IE_MoveCard(int p_touchCardPositionIndex){
		prePlayerPositionIndex = playerCard.GetPositionIndex ();
		StartCoroutine (playerCard.IE_SetPosition (p_touchCardPositionIndex));

		if (prePlayerPositionIndex - p_touchCardPositionIndex == 1) {
			cardMoveState = CardMoveType.Left;
		}
		else if (prePlayerPositionIndex - p_touchCardPositionIndex == -1) {
			cardMoveState = CardMoveType.Right;
		}
		else if (prePlayerPositionIndex - p_touchCardPositionIndex == 3) {
			cardMoveState = CardMoveType.Up;
		}
		else if (prePlayerPositionIndex - p_touchCardPositionIndex == -3) {
			cardMoveState = CardMoveType.Down;
		}

		switch (cardMoveState) {
		case CardMoveType.Down:
			if(prePlayerPositionIndex - 3 >= 0)
				StartCoroutine(GetCard (prePlayerPositionIndex - 3).IE_SetPosition (prePlayerPositionIndex));
			break;

		case CardMoveType.Up:
			if (prePlayerPositionIndex + 3 <= 8)
				StartCoroutine(GetCard (prePlayerPositionIndex + 3).IE_SetPosition (prePlayerPositionIndex));
			break;

		case CardMoveType.Left:
			if (prePlayerPositionIndex % 3 != 2)
				StartCoroutine(GetCard (prePlayerPositionIndex + 1).IE_SetPosition (prePlayerPositionIndex));
			break;

		case CardMoveType.Right:
			if (prePlayerPositionIndex % 3 != 0)
				StartCoroutine(GetCard (prePlayerPositionIndex-1).IE_SetPosition (prePlayerPositionIndex));
			break;
		}
		yield return new WaitForSeconds(0.5f);
	}

	IEnumerator IE_DestoryCard(int p_positionIndex){
		for(int i=0; i< cards.Count; i++){
			if(cards[i].GetPositionIndex() == p_positionIndex){
				yield return StartCoroutine(cards[i].Destory());
				cardsPool.Enqueue(cards[i]);
				cards[i].transform.SetParent(cardPoolTransform, false);
				cards.RemoveAt (i);
			}
		}
	}

	private void DoCardAction(CardPoker p_card){
		switch (p_card.GetCardType()) {
		case "Poker":
			//將排放入底下
			//做判斷
			//
			break;
		case "Action":
			break;
		case "Base":
			break;
		case "Question":
			break;
		case "Marvel":
			break;
		case "ChangeOperation":
			break;
		case "Null":
			break;
		}
	}

	private bool IsNearby(int p_playerPositionIndex, int p_touchCardPositionIndex){
		bool _isNearby = false;

		if ((p_touchCardPositionIndex == p_playerPositionIndex - 1 && p_playerPositionIndex%3 !=  0)|| //左邊
			(p_touchCardPositionIndex == p_playerPositionIndex + 1 && p_playerPositionIndex%3 != 2)|| //右邊
			p_touchCardPositionIndex == p_playerPositionIndex - 3 || //上面
			p_touchCardPositionIndex == p_playerPositionIndex + 3 //下面
		) {
			_isNearby = true;
		}

		return _isNearby;
	}
}