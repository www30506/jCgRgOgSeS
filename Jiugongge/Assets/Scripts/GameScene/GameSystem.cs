using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameSystem : MonoBehaviour {
	private enum OperationType{Addition, Subtraction, Multiplication, Division};
	private enum SystemStatue{Idle, Working, Win , Loss};
	private enum CardMoveType{Left, Right, Up, Down};
	[SerializeField]private CardMoveType cardMoveState = CardMoveType.Left;

	[Header("Data")]
	[SerializeField]private int[] completeTargets;
	[SerializeField]private bool[] IscompleteTargets = new bool[3];
	[SerializeField]private OperationType operationState= OperationType.Addition;
	[SerializeField]private string[] initCardsID;
	[SerializeField]private string[] drawCardsList;
	[SerializeField]private float actionValue;
	[Header("====")]

	[SerializeField]private int drawCardIndex;

	[SerializeField]private int maxCards = 9;
	[SerializeField]private List<Card> cards;
	private Queue<Card> cardsPool;
	[SerializeField]private Transform cardListTransform;
	[SerializeField]private Transform cardPoolTransform;
	[SerializeField]private GameView gameView;
	private SystemStatue systemStatue = SystemStatue.Idle;
	[SerializeField]private Card playerCard;
	private int prePlayerPositionIndex;

	void Start () {
//		PlayerPrefs.SetInt ("Action",100);
		LoadLevelData();
		CreateCardPool();
		CreateCards();
		CreateCompleteTarget ();
		InitActionValue ();
	}

	private void InitActionValue(){
//		int _isFirstGame = PlayerPrefs.GetInt ("FirstGame");

//		if (_isFirstGame == 0) {
//			PlayerPrefs.SetInt ("FirstGame", 1);
//			PlayerPrefs.SetInt ("Action",100);
//		}

//		actionValue = PlayerPrefs.GetInt ("Action");
		actionValue = 10;
		gameView.SetActionValue (actionValue);
	}

//	private bool IsActionValueEnough(){
//		if (actionValue > 0)
//			return true;
//		return false;
//	}

	private void SetActionValue(int p_value){
		actionValue += p_value;
//		PlayerPrefs.SetInt ("Action", actionValue);
		gameView.SetActionValue (actionValue);
	}

	private void CreateCompleteTarget(){
		completeTargets = new int[3];

		List<int> _range = new List<int> ();
		for (int i = 1; i < 4; i++) {
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

	private Card GetCard(int p_positionIndex){
		for(int i=0; i< maxCards; i++){
			if(cards[i].GetPositionIndex() == p_positionIndex){
				return cards[i];
			}
		}
		Debug.LogError("錯誤 沒有取得卡片 位置編號 : " + p_positionIndex);
		return null;
	}

	IEnumerator CreateCard(string p_CardID, int p_positionIndex){
		Card _card = cardsPool.Dequeue();
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
			StartCoroutine(CreateCard(initCardsID[i], i));
		}
	}

	void Update () {
		actionValue -= Time.deltaTime;
		gameView.SetActionValue (actionValue);
		if (actionValue < 0 && systemStatue != SystemStatue.Loss) {
			print ("【遊戲結束】 時間結束");
			systemStatue = SystemStatue.Loss;
			gameView.ShowLossUI ();

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

//		if (IsActionValueEnough ()) {
//			SetActionValue (-1);
			StartCoroutine (IE_OnTouchCardEvent (p_touchCardPositionIndex));
//		}
//		else {
//			print ("行動不足");
//		}

	}

	IEnumerator IE_OnTouchCardEvent(int p_touchCardPositionIndex){
		systemStatue = SystemStatue.Working;

		if (IsNearby (playerCard.GetPositionIndex (), p_touchCardPositionIndex)) {
			Card _card = GetCard (p_touchCardPositionIndex);

			DoCardAction (_card);

			yield return StartCoroutine(IE_DestoryCard(p_touchCardPositionIndex));

			yield return StartCoroutine (IE_MoveCard (p_touchCardPositionIndex));

			yield return StartCoroutine(CreateCard (drawCardsList [drawCardIndex], GetCreateCardPositionIndex()));

			drawCardIndex++;

			if (drawCardIndex > drawCardsList.Length-1) {
				drawCardIndex = 0;
			}

			CheckCompleteTarget ();

			if (IsWinTheGame () && systemStatue != SystemStatue.Win) {
				print ("勝利");
				systemStatue = SystemStatue.Win;
				gameView.ShowWinUI ();
			}
		}

		systemStatue = SystemStatue.Idle;
		yield return null;
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

	private void DoCardAction(Card p_card){
		switch (p_card.GetCardType()) {
		case "Action":
			SetActionValue(p_card.GetCardValue ());
			break;
		case "Base":
			if (operationState == OperationType.Addition) {
				playerCard.AdditionValue (p_card.GetCardValue ());
			} 
			else if (operationState == OperationType.Subtraction) {
				playerCard.SubtractionValue (p_card.GetCardValue ());
			}
			else if (operationState == OperationType.Multiplication) {
				playerCard.MultiplicationValue (p_card.GetCardValue ());
			}
			else if (operationState == OperationType.Division) {
				if (playerCard.CanDivision (p_card.GetCardValue ())) {
					playerCard.DivisionValue (p_card.GetCardValue ());
				}
				else {
					print ("不能執行");
				}
			}
			break;
		case "Null":
			break;
		}
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

	public void OnChangeOperation(string p_Operation){
		operationState = (OperationType)Enum.Parse (typeof(OperationType), p_Operation);
	}

	private void CheckCompleteTarget(){
		for (int i = 0; i < IscompleteTargets.Length; i++) {
			if (playerCard.GetCardValue () == completeTargets [i] && IscompleteTargets [i] == false) {
				IscompleteTargets [i] = true;
				gameView.CompleteTargetEff (i);
			}
		}
	}

	private bool IsWinTheGame(){
		for (int i = 0; i < IscompleteTargets.Length; i++) {
			if (IscompleteTargets [i] == false) {
				return false;
			}
		}

		return true;
	}

	public void WinTheGame(){
//		int _actionValue = PlayerPrefs.GetInt ("Action");
//		PlayerPrefs.SetInt ("Action", _actionValue+10);
		BackToMenu ();
	}
}
